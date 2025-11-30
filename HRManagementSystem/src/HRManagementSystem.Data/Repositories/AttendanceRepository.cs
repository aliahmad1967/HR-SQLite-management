// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// AttendanceRepository.cs - مستودع الحضور
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع الحضور
/// Attendance repository
/// </summary>
public class AttendanceRepository : BaseRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(IDatabaseContext context) : base(context, "Attendance")
    {
    }

    /// <summary>
    /// جلب حضور اليوم للموظف
    /// Get today's attendance for employee
    /// </summary>
    public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId)
    {
        var sql = "SELECT * FROM Attendance WHERE EmployeeId = @EmployeeId AND Date = date('now')";
        return await _context.Connection.QueryFirstOrDefaultAsync<Attendance>(sql, 
            new { EmployeeId = employeeId }, _context.Transaction);
    }

    /// <summary>
    /// جلب الحضور لفترة معينة
    /// Get attendance by date range
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate)
    {
        var sql = @"SELECT * FROM Attendance 
                    WHERE EmployeeId = @EmployeeId 
                    AND Date BETWEEN @StartDate AND @EndDate
                    ORDER BY Date DESC";
        return await _context.Connection.QueryAsync<Attendance>(sql, 
            new { EmployeeId = employeeId, StartDate = startDate.Date, EndDate = endDate.Date }, 
            _context.Transaction);
    }

    /// <summary>
    /// جلب الحضور ليوم معين
    /// Get attendance by date
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date)
    {
        var sql = "SELECT * FROM Attendance WHERE Date = @Date ORDER BY CheckInTime";
        return await _context.Connection.QueryAsync<Attendance>(sql, 
            new { Date = date.Date }, _context.Transaction);
    }

    /// <summary>
    /// جلب حضور قسم معين
    /// Get department attendance
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetDepartmentAttendanceAsync(int departmentId, DateTime date)
    {
        var sql = @"SELECT a.* FROM Attendance a
                    INNER JOIN Employees e ON a.EmployeeId = e.Id
                    WHERE e.DepartmentId = @DepartmentId AND a.Date = @Date
                    ORDER BY a.CheckInTime";
        return await _context.Connection.QueryAsync<Attendance>(sql, 
            new { DepartmentId = departmentId, Date = date.Date }, _context.Transaction);
    }

    /// <summary>
    /// تسجيل الحضور
    /// Check in
    /// </summary>
    public async Task<bool> CheckInAsync(int employeeId)
    {
        var existing = await GetTodayAttendanceAsync(employeeId);
        if (existing != null)
            return false;

        var sql = @"INSERT INTO Attendance (EmployeeId, Date, CheckInTime, Status)
                    VALUES (@EmployeeId, date('now'), time('now'), 'حاضر');";
        var rows = await _context.Connection.ExecuteAsync(sql, 
            new { EmployeeId = employeeId }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// تسجيل الانصراف
    /// Check out
    /// </summary>
    public async Task<bool> CheckOutAsync(int employeeId)
    {
        var sql = @"UPDATE Attendance SET CheckOutTime = time('now')
                    WHERE EmployeeId = @EmployeeId AND Date = date('now') AND CheckOutTime IS NULL";
        var rows = await _context.Connection.ExecuteAsync(sql, 
            new { EmployeeId = employeeId }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// جلب إحصائيات الشهر
    /// Get monthly statistics
    /// </summary>
    public async Task<Dictionary<string, int>> GetMonthlyStatsAsync(int employeeId, int year, int month)
    {
        var sql = @"SELECT Status, COUNT(*) as Count FROM Attendance 
                    WHERE EmployeeId = @EmployeeId 
                    AND strftime('%Y', Date) = @Year 
                    AND strftime('%m', Date) = @Month
                    GROUP BY Status";
        
        var results = await _context.Connection.QueryAsync<(string Status, int Count)>(sql, 
            new { EmployeeId = employeeId, Year = year.ToString(), Month = month.ToString("D2") }, 
            _context.Transaction);

        return results.ToDictionary(r => r.Status, r => r.Count);
    }

    /// <summary>
    /// إضافة سجل حضور
    /// Add attendance record
    /// </summary>
    public override async Task<int> AddAsync(Attendance entity)
    {
        var sql = @"INSERT INTO Attendance (EmployeeId, Date, CheckInTime, CheckOutTime, Status, Notes)
                    VALUES (@EmployeeId, @Date, @CheckInTime, @CheckOutTime, @Status, @Notes);
                    SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }
}

