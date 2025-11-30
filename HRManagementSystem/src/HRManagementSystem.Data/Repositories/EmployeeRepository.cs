// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// EmployeeRepository.cs - مستودع الموظفين
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع الموظفين
/// Employee repository
/// </summary>
public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(IDatabaseContext context) : base(context, "Employees")
    {
    }

    /// <summary>
    /// جلب موظف برقم الموظف
    /// Get employee by employee number
    /// </summary>
    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        var sql = "SELECT * FROM Employees WHERE EmployeeNumber = @EmployeeNumber";
        return await _context.Connection.QueryFirstOrDefaultAsync<Employee>(sql, 
            new { EmployeeNumber = employeeNumber }, _context.Transaction);
    }

    /// <summary>
    /// جلب موظف برقم الهوية
    /// Get employee by national ID
    /// </summary>
    public async Task<Employee?> GetByNationalIdAsync(string nationalId)
    {
        var sql = "SELECT * FROM Employees WHERE NationalId = @NationalId";
        return await _context.Connection.QueryFirstOrDefaultAsync<Employee>(sql, 
            new { NationalId = nationalId }, _context.Transaction);
    }

    /// <summary>
    /// جلب موظفي قسم معين
    /// Get employees by department
    /// </summary>
    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
    {
        var sql = @"SELECT e.*, d.* FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE e.DepartmentId = @DepartmentId AND e.IsActive = 1";
        return await _context.Connection.QueryAsync<Employee, Department, Employee>(sql,
            (emp, dept) => { emp.Department = dept; return emp; },
            new { DepartmentId = departmentId }, _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// جلب الموظفين النشطين
    /// Get active employees
    /// </summary>
    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        var sql = @"SELECT e.*, d.* FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE e.IsActive = 1";
        return await _context.Connection.QueryAsync<Employee, Department, Employee>(sql,
            (emp, dept) => { emp.Department = dept; return emp; },
            transaction: _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// البحث عن موظفين
    /// Search employees
    /// </summary>
    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
    {
        var sql = @"SELECT e.*, d.* FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE (e.FirstName LIKE @Term OR e.LastName LIKE @Term
                    OR e.EmployeeNumber LIKE @Term OR e.NationalId LIKE @Term
                    OR e.Phone LIKE @Term OR e.Email LIKE @Term)
                    AND e.IsActive = 1";
        return await _context.Connection.QueryAsync<Employee, Department, Employee>(sql,
            (emp, dept) => { emp.Department = dept; return emp; },
            new { Term = $"%{searchTerm}%" }, _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// توليد رقم موظف جديد
    /// Generate new employee number
    /// </summary>
    public async Task<string> GenerateEmployeeNumberAsync()
    {
        var sql = "SELECT MAX(CAST(SUBSTR(EmployeeNumber, 4) AS INTEGER)) FROM Employees WHERE EmployeeNumber LIKE 'EMP%'";
        var maxNumber = await _context.Connection.ExecuteScalarAsync<int?>(sql, transaction: _context.Transaction);
        var newNumber = (maxNumber ?? 0) + 1;
        return $"EMP{newNumber:D6}";
    }

    /// <summary>
    /// جلب موظف مع التفاصيل الكاملة
    /// Get employee with full details
    /// </summary>
    public async Task<Employee?> GetWithDetailsAsync(int id)
    {
        var sql = @"
            SELECT e.*, d.*, p.*, m.*
            FROM Employees e
            LEFT JOIN Departments d ON e.DepartmentId = d.Id
            LEFT JOIN Positions p ON e.PositionId = p.Id
            LEFT JOIN Employees m ON e.ManagerId = m.Id
            WHERE e.Id = @Id";

        var employees = await _context.Connection.QueryAsync<Employee, Department, Position, Employee, Employee>(
            sql,
            (employee, department, position, manager) =>
            {
                employee.Department = department;
                employee.Position = position;
                employee.Manager = manager;
                return employee;
            },
            new { Id = id },
            _context.Transaction,
            splitOn: "Id,Id,Id"
        );

        return employees.FirstOrDefault();
    }

    /// <summary>
    /// إضافة موظف جديد مع توليد الرقم
    /// Add new employee with number generation
    /// </summary>
    public override async Task<int> AddAsync(Employee entity)
    {
        if (string.IsNullOrEmpty(entity.EmployeeNumber))
        {
            entity.EmployeeNumber = await GenerateEmployeeNumberAsync();
        }

        var sql = @"INSERT INTO Employees (
            EmployeeNumber, FirstName, LastName,
            NationalId, Gender, DateOfBirth,
            Email, Phone, Address, City,
            DepartmentId, PositionId, ManagerId, HireDate,
            EmploymentStatus, BasicSalary, IsActive
        ) VALUES (
            @EmployeeNumber, @FirstName, @LastName,
            @NationalId, @Gender, @DateOfBirth,
            @Email, @Phone, @Address, @City,
            @DepartmentId, @PositionId, @ManagerId, @HireDate,
            @EmploymentStatus, @BasicSalary, @IsActive
        ); SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }

    /// <summary>
    /// تحديث موظف
    /// Update employee
    /// </summary>
    public override async Task<bool> UpdateAsync(Employee entity)
    {
        var sql = @"UPDATE Employees SET
            FirstName = @FirstName, LastName = @LastName,
            NationalId = @NationalId, Gender = @Gender, DateOfBirth = @DateOfBirth,
            Email = @Email, Phone = @Phone, Address = @Address, City = @City,
            DepartmentId = @DepartmentId, PositionId = @PositionId, ManagerId = @ManagerId,
            EmploymentStatus = @EmploymentStatus, BasicSalary = @BasicSalary,
            IsActive = @IsActive, UpdatedAt = datetime('now')
            WHERE Id = @Id";

        var rows = await _context.Connection.ExecuteAsync(sql, entity, _context.Transaction);
        return rows > 0;
    }
}

