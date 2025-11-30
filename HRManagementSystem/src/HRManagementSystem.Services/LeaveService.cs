// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// LeaveService.cs - خدمة الإجازات
// =====================================================

using HRManagementSystem.Core.Enums;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Serilog;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة الإجازات
/// Leave service
/// </summary>
public class LeaveService : ILeaveService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public LeaveService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = Log.ForContext<LeaveService>();
    }

    /// <summary>
    /// جلب إجازات موظف
    /// Get employee leaves
    /// </summary>
    public async Task<IEnumerable<Leave>> GetByEmployeeAsync(int employeeId)
    {
        return await _unitOfWork.Leaves.GetByEmployeeAsync(employeeId);
    }

    /// <summary>
    /// جلب الإجازات المعلقة
    /// Get pending leaves
    /// </summary>
    public async Task<IEnumerable<Leave>> GetPendingLeavesAsync()
    {
        return await _unitOfWork.Leaves.GetPendingLeavesAsync();
    }

    /// <summary>
    /// جلب أرصدة الإجازات
    /// Get leave balances
    /// </summary>
    public async Task<IEnumerable<LeaveBalance>> GetBalancesAsync(int employeeId, int year)
    {
        return await _unitOfWork.Leaves.GetBalancesAsync(employeeId, year);
    }

    /// <summary>
    /// جلب أنواع الإجازات
    /// Get leave types
    /// </summary>
    public async Task<IEnumerable<LeaveType>> GetLeaveTypesAsync()
    {
        return await _unitOfWork.Leaves.GetLeaveTypesAsync();
    }

    /// <summary>
    /// طلب إجازة
    /// Request leave
    /// </summary>
    public async Task<int> RequestLeaveAsync(Leave leave)
    {
        try
        {
            // التحقق من صحة الطلب
            var validation = await ValidateLeaveRequestAsync(leave);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(validation.Message);
            }

            // حساب عدد الأيام
            leave.TotalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
            leave.Status = LeaveStatus.Pending;

            var id = await _unitOfWork.Leaves.AddAsync(leave);
            _logger.Information("تم تقديم طلب إجازة: {LeaveId} - الموظف: {EmployeeId}", id, leave.EmployeeId);

            return id;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تقديم طلب إجازة");
            throw;
        }
    }

    /// <summary>
    /// الموافقة على إجازة
    /// Approve leave
    /// </summary>
    public async Task<bool> ApproveLeaveAsync(int leaveId, int approverId)
    {
        try
        {
            var result = await _unitOfWork.Leaves.ApproveLeaveAsync(leaveId, approverId);
            if (result)
                _logger.Information("تمت الموافقة على الإجازة: {LeaveId}", leaveId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء الموافقة على الإجازة: {LeaveId}", leaveId);
            throw;
        }
    }

    /// <summary>
    /// رفض إجازة
    /// Reject leave
    /// </summary>
    public async Task<bool> RejectLeaveAsync(int leaveId, int approverId, string reason)
    {
        try
        {
            var result = await _unitOfWork.Leaves.RejectLeaveAsync(leaveId, approverId, reason);
            if (result)
                _logger.Information("تم رفض الإجازة: {LeaveId} - السبب: {Reason}", leaveId, reason);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء رفض الإجازة: {LeaveId}", leaveId);
            throw;
        }
    }

    /// <summary>
    /// إلغاء إجازة
    /// Cancel leave
    /// </summary>
    public async Task<bool> CancelLeaveAsync(int leaveId)
    {
        try
        {
            var leave = await _unitOfWork.Leaves.GetByIdAsync(leaveId);
            if (leave == null || leave.Status != LeaveStatus.Pending)
                return false;

            leave.Status = LeaveStatus.Cancelled;
            var result = await _unitOfWork.Leaves.UpdateAsync(leave);

            if (result)
                _logger.Information("تم إلغاء الإجازة: {LeaveId}", leaveId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء إلغاء الإجازة: {LeaveId}", leaveId);
            throw;
        }
    }

    /// <summary>
    /// التحقق من صحة طلب الإجازة
    /// Validate leave request
    /// </summary>
    public async Task<(bool IsValid, string Message)> ValidateLeaveRequestAsync(Leave leave)
    {
        // التحقق من وجود موظف ونوع إجازة
        if (leave.EmployeeId <= 0)
            return (false, "يجب اختيار الموظف");

        if (leave.LeaveTypeId <= 0)
            return (false, "يجب اختيار نوع الإجازة");

        // التحقق من التواريخ
        if (leave.StartDate > leave.EndDate)
            return (false, "تاريخ البداية يجب أن يكون قبل تاريخ النهاية");

        // التحقق من التداخل (اختياري)
        try
        {
            var hasOverlap = await _unitOfWork.Leaves.HasOverlappingLeaveAsync(
                leave.EmployeeId, leave.StartDate, leave.EndDate);
            if (hasOverlap)
                return (false, "يوجد تداخل مع إجازة أخرى");
        }
        catch
        {
            // تجاهل أخطاء التحقق من التداخل
        }

        return (true, "الطلب صالح");
    }
}

