// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// LeaveRepository.cs - مستودع الإجازات
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع الإجازات
/// Leave repository
/// </summary>
public class LeaveRepository : BaseRepository<Leave>, ILeaveRepository
{
    public LeaveRepository(IDatabaseContext context) : base(context, "Leaves")
    {
    }

    /// <summary>
    /// جلب إجازات موظف
    /// Get employee leaves
    /// </summary>
    public async Task<IEnumerable<Leave>> GetByEmployeeAsync(int employeeId)
    {
        var sql = @"SELECT l.*, lt.* FROM Leaves l
                    INNER JOIN LeaveTypes lt ON l.LeaveTypeId = lt.Id
                    WHERE l.EmployeeId = @EmployeeId
                    ORDER BY l.StartDate DESC";
        
        return await _context.Connection.QueryAsync<Leave, LeaveType, Leave>(sql,
            (leave, leaveType) => { leave.LeaveType = leaveType; return leave; },
            new { EmployeeId = employeeId }, _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// جلب الإجازات المعلقة
    /// Get pending leaves
    /// </summary>
    public async Task<IEnumerable<Leave>> GetPendingLeavesAsync()
    {
        var sql = @"SELECT l.*, e.*, lt.* FROM Leaves l
                    INNER JOIN Employees e ON l.EmployeeId = e.Id
                    INNER JOIN LeaveTypes lt ON l.LeaveTypeId = lt.Id
                    WHERE l.Status = 'معلق'
                    ORDER BY l.CreatedAt";
        
        return await _context.Connection.QueryAsync<Leave, Employee, LeaveType, Leave>(sql,
            (leave, employee, leaveType) => 
            { 
                leave.Employee = employee; 
                leave.LeaveType = leaveType; 
                return leave; 
            },
            transaction: _context.Transaction, splitOn: "Id,Id");
    }

    /// <summary>
    /// جلب أرصدة الإجازات
    /// Get leave balances
    /// </summary>
    public async Task<IEnumerable<LeaveBalance>> GetBalancesAsync(int employeeId, int year)
    {
        var sql = @"SELECT lb.*, lt.* FROM LeaveBalances lb
                    INNER JOIN LeaveTypes lt ON lb.LeaveTypeId = lt.Id
                    WHERE lb.EmployeeId = @EmployeeId AND lb.Year = @Year";
        
        return await _context.Connection.QueryAsync<LeaveBalance, LeaveType, LeaveBalance>(sql,
            (balance, leaveType) => { balance.LeaveType = leaveType; return balance; },
            new { EmployeeId = employeeId, Year = year }, _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// الموافقة على إجازة
    /// Approve leave
    /// </summary>
    public async Task<bool> ApproveLeaveAsync(int leaveId, int approverId)
    {
        var sql = @"UPDATE Leaves SET Status = 'موافق', ApprovedBy = @ApproverId, 
                    ApprovedAt = datetime('now'), UpdatedAt = datetime('now')
                    WHERE Id = @LeaveId AND Status = 'معلق'";
        var rows = await _context.Connection.ExecuteAsync(sql, 
            new { LeaveId = leaveId, ApproverId = approverId }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// رفض إجازة
    /// Reject leave
    /// </summary>
    public async Task<bool> RejectLeaveAsync(int leaveId, int approverId, string reason)
    {
        var sql = @"UPDATE Leaves SET Status = 'مرفوض', ApprovedBy = @ApproverId, 
                    RejectionReason = @Reason, ApprovedAt = datetime('now'), UpdatedAt = datetime('now')
                    WHERE Id = @LeaveId AND Status = 'معلق'";
        var rows = await _context.Connection.ExecuteAsync(sql, 
            new { LeaveId = leaveId, ApproverId = approverId, Reason = reason }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// جلب أنواع الإجازات
    /// Get leave types
    /// </summary>
    public async Task<IEnumerable<LeaveType>> GetLeaveTypesAsync()
    {
        var sql = "SELECT * FROM LeaveTypes WHERE IsActive = 1 ORDER BY Name";
        return await _context.Connection.QueryAsync<LeaveType>(sql, transaction: _context.Transaction);
    }

    /// <summary>
    /// التحقق من تداخل الإجازات
    /// Check overlapping leaves
    /// </summary>
    public async Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveId = null)
    {
        var sql = @"SELECT COUNT(*) FROM Leaves 
                    WHERE EmployeeId = @EmployeeId 
                    AND Status IN ('معلق', 'موافق')
                    AND ((StartDate <= @EndDate AND EndDate >= @StartDate))
                    AND (@ExcludeId IS NULL OR Id != @ExcludeId)";
        
        var count = await _context.Connection.ExecuteScalarAsync<int>(sql, 
            new { EmployeeId = employeeId, StartDate = startDate, EndDate = endDate, ExcludeId = excludeLeaveId }, 
            _context.Transaction);
        return count > 0;
    }

    /// <summary>
    /// إضافة طلب إجازة
    /// Add leave request
    /// </summary>
    public override async Task<int> AddAsync(Leave entity)
    {
        var sql = @"INSERT INTO Leaves (EmployeeId, LeaveTypeId, StartDate, EndDate, TotalDays, Reason, Status)
                    VALUES (@EmployeeId, @LeaveTypeId, @StartDate, @EndDate, @TotalDays, @Reason, 'Pending');
                    SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }
}

