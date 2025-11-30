// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// AttendanceService.cs - خدمة الحضور
// =====================================================

using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Serilog;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة الحضور
/// Attendance service
/// </summary>
public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public AttendanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = Log.ForContext<AttendanceService>();
    }

    /// <summary>
    /// تسجيل الحضور
    /// Check in
    /// </summary>
    public async Task<bool> CheckInAsync(int employeeId)
    {
        try
        {
            var existing = await _unitOfWork.Attendance.GetTodayAttendanceAsync(employeeId);
            if (existing != null)
            {
                _logger.Warning("الموظف سجل حضوره مسبقاً اليوم: {EmployeeId}", employeeId);
                return false;
            }

            var result = await _unitOfWork.Attendance.CheckInAsync(employeeId);
            if (result)
                _logger.Information("تم تسجيل حضور الموظف: {EmployeeId}", employeeId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تسجيل الحضور: {EmployeeId}", employeeId);
            throw;
        }
    }

    /// <summary>
    /// تسجيل الانصراف
    /// Check out
    /// </summary>
    public async Task<bool> CheckOutAsync(int employeeId)
    {
        try
        {
            var existing = await _unitOfWork.Attendance.GetTodayAttendanceAsync(employeeId);
            if (existing == null)
            {
                _logger.Warning("لا يوجد سجل حضور للموظف اليوم: {EmployeeId}", employeeId);
                return false;
            }

            if (existing.CheckOutTime.HasValue)
            {
                _logger.Warning("الموظف سجل انصرافه مسبقاً: {EmployeeId}", employeeId);
                return false;
            }

            var result = await _unitOfWork.Attendance.CheckOutAsync(employeeId);
            if (result)
                _logger.Information("تم تسجيل انصراف الموظف: {EmployeeId}", employeeId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تسجيل الانصراف: {EmployeeId}", employeeId);
            throw;
        }
    }

    /// <summary>
    /// جلب حضور اليوم
    /// Get today's attendance
    /// </summary>
    public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId)
    {
        return await _unitOfWork.Attendance.GetTodayAttendanceAsync(employeeId);
    }

    /// <summary>
    /// جلب الحضور لفترة معينة
    /// Get attendance by date range
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.Attendance.GetByDateRangeAsync(employeeId, startDate, endDate);
    }

    /// <summary>
    /// جلب حضور قسم معين
    /// Get department attendance
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetDepartmentAttendanceAsync(int departmentId, DateTime date)
    {
        return await _unitOfWork.Attendance.GetDepartmentAttendanceAsync(departmentId, date);
    }

    /// <summary>
    /// جلب إحصائيات الشهر
    /// Get monthly statistics
    /// </summary>
    public async Task<Dictionary<string, int>> GetMonthlyStatsAsync(int employeeId, int year, int month)
    {
        return await _unitOfWork.Attendance.GetMonthlyStatsAsync(employeeId, year, month);
    }

    /// <summary>
    /// إضافة حضور يدوي
    /// Add manual attendance
    /// </summary>
    public async Task<bool> AddManualAttendanceAsync(Attendance attendance)
    {
        try
        {
            // التحقق من عدم وجود سجل لنفس اليوم
            var existing = await _unitOfWork.Attendance.GetByDateRangeAsync(
                attendance.EmployeeId, attendance.AttendanceDate, attendance.AttendanceDate);

            if (existing.Any())
            {
                _logger.Warning("يوجد سجل حضور لهذا اليوم: {EmployeeId} - {Date}",
                    attendance.EmployeeId, attendance.AttendanceDate);
                return false;
            }

            var id = await _unitOfWork.Attendance.AddAsync(attendance);
            _logger.Information("تم إضافة سجل حضور يدوي: {EmployeeId} - {Date}",
                attendance.EmployeeId, attendance.AttendanceDate);

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء إضافة سجل حضور يدوي");
            throw;
        }
    }
}

