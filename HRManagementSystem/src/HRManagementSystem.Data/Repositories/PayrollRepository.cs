// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// PayrollRepository.cs - مستودع الرواتب
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع الرواتب
/// Payroll repository
/// </summary>
public class PayrollRepository : BaseRepository<Payroll>, IPayrollRepository
{
    public PayrollRepository(IDatabaseContext context) : base(context, "Payroll")
    {
    }

    /// <summary>
    /// تحديث راتب
    /// Update payroll
    /// </summary>
    public override async Task<bool> UpdateAsync(Payroll entity)
    {
        var sql = @"UPDATE Payroll SET
            BasicSalary = @BasicSalary, TotalAllowances = @TotalAllowances,
            TotalDeductions = @TotalDeductions, OvertimeAmount = @OvertimeAmount,
            GrossSalary = @GrossSalary, NetSalary = @NetSalary,
            Status = @Status, Notes = @Notes, UpdatedAt = datetime('now')
            WHERE Id = @Id";
        var rows = await _context.Connection.ExecuteAsync(sql, entity, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// جلب راتب موظف لفترة معينة
    /// Get employee payroll by period
    /// </summary>
    public async Task<Payroll?> GetByPeriodAsync(int employeeId, int year, int month)
    {
        var sql = "SELECT * FROM Payroll WHERE EmployeeId = @EmployeeId AND Year = @Year AND Month = @Month";
        return await _context.Connection.QueryFirstOrDefaultAsync<Payroll>(sql, 
            new { EmployeeId = employeeId, Year = year, Month = month }, _context.Transaction);
    }

    /// <summary>
    /// جلب رواتب فترة معينة
    /// Get payroll by period
    /// </summary>
    public async Task<IEnumerable<Payroll>> GetByPeriodAsync(int year, int month)
    {
        var sql = @"SELECT p.*, e.* FROM Payroll p
                    INNER JOIN Employees e ON p.EmployeeId = e.Id
                    WHERE p.Year = @Year AND p.Month = @Month
                    ORDER BY e.FirstName, e.LastName";

        return await _context.Connection.QueryAsync<Payroll, Employee, Payroll>(sql,
            (payroll, employee) => { payroll.Employee = employee; return payroll; },
            new { Year = year, Month = month }, _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// جلب راتب مع التفاصيل
    /// Get payroll with details
    /// </summary>
    public async Task<Payroll?> GetWithDetailsAsync(int id)
    {
        var sql = @"SELECT p.*, e.*, pd.* FROM Payroll p
                    INNER JOIN Employees e ON p.EmployeeId = e.Id
                    LEFT JOIN PayrollDetails pd ON p.Id = pd.PayrollId
                    WHERE p.Id = @Id";

        var payrollDict = new Dictionary<int, Payroll>();

        await _context.Connection.QueryAsync<Payroll, Employee, PayrollDetail, Payroll>(sql,
            (payroll, employee, detail) =>
            {
                if (!payrollDict.TryGetValue(payroll.Id, out var currentPayroll))
                {
                    currentPayroll = payroll;
                    currentPayroll.Employee = employee;
                    currentPayroll.Details = new List<PayrollDetail>();
                    payrollDict.Add(currentPayroll.Id, currentPayroll);
                }

                if (detail != null)
                {
                    ((List<PayrollDetail>)currentPayroll.Details).Add(detail);
                }

                return currentPayroll;
            },
            new { Id = id }, _context.Transaction, splitOn: "Id,Id");

        return payrollDict.Values.FirstOrDefault();
    }

    /// <summary>
    /// توليد رواتب الشهر
    /// Generate monthly payroll
    /// </summary>
    public async Task<bool> GenerateMonthlyPayrollAsync(int year, int month, int createdBy)
    {
        var sql = @"INSERT INTO Payroll (EmployeeId, Year, Month, BasicSalary, Status)
                    SELECT Id, @Year, @Month, BasicSalary, 'Draft'
                    FROM Employees
                    WHERE IsActive = 1
                    AND Id NOT IN (SELECT EmployeeId FROM Payroll WHERE Year = @Year AND Month = @Month)";

        var rows = await _context.Connection.ExecuteAsync(sql,
            new { Year = year, Month = month }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// اعتماد الراتب
    /// Approve payroll
    /// </summary>
    public async Task<bool> ApprovePayrollAsync(int payrollId, int approverId)
    {
        var sql = @"UPDATE Payroll SET Status = 'Processed',
                    ProcessedAt = datetime('now'), ProcessedById = @ApproverId, UpdatedAt = datetime('now')
                    WHERE Id = @PayrollId AND Status = 'Draft'";
        var rows = await _context.Connection.ExecuteAsync(sql,
            new { PayrollId = payrollId, ApproverId = approverId }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// حساب العمل الإضافي
    /// Calculate overtime
    /// </summary>
    public async Task<decimal> CalculateOvertimeAsync(int employeeId, int year, int month, decimal hourlyRate)
    {
        var sql = @"SELECT COALESCE(SUM(OvertimeHours), 0) FROM Attendance 
                    WHERE EmployeeId = @EmployeeId 
                    AND strftime('%Y', Date) = @Year 
                    AND strftime('%m', Date) = @Month";
        
        var hours = await _context.Connection.ExecuteScalarAsync<decimal>(sql, 
            new { EmployeeId = employeeId, Year = year.ToString(), Month = month.ToString("D2") }, 
            _context.Transaction);
        return hours * hourlyRate * 1.5m; // 150% للعمل الإضافي
    }

    /// <summary>
    /// جلب عناصر الراتب
    /// Get salary components
    /// </summary>
    public async Task<IEnumerable<SalaryComponent>> GetSalaryComponentsAsync()
    {
        // جدول SalaryComponents قد لا يكون موجوداً
        try
        {
            var sql = "SELECT * FROM SalaryComponents WHERE IsActive = 1 ORDER BY SortOrder";
            return await _context.Connection.QueryAsync<SalaryComponent>(sql, transaction: _context.Transaction);
        }
        catch
        {
            return Enumerable.Empty<SalaryComponent>();
        }
    }

    /// <summary>
    /// جلب عناصر راتب الموظف
    /// Get employee salary components
    /// </summary>
    public async Task<IEnumerable<EmployeeSalaryComponent>> GetEmployeeSalaryComponentsAsync(int employeeId)
    {
        try
        {
            var sql = @"SELECT esc.*, sc.* FROM EmployeeSalaryComponents esc
                        INNER JOIN SalaryComponents sc ON esc.SalaryComponentId = sc.Id
                        WHERE esc.EmployeeId = @EmployeeId AND esc.IsActive = 1
                        AND (esc.EndDate IS NULL OR esc.EndDate >= date('now'))";

            return await _context.Connection.QueryAsync<EmployeeSalaryComponent, SalaryComponent, EmployeeSalaryComponent>(sql,
                (esc, sc) => { esc.SalaryComponent = sc; return esc; },
                new { EmployeeId = employeeId }, _context.Transaction, splitOn: "Id");
        }
        catch
        {
            return Enumerable.Empty<EmployeeSalaryComponent>();
        }
    }
}

