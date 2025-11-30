// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// LeavesViewModel.cs - ViewModel الإجازات
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الإجازات
/// Leaves ViewModel
/// </summary>
public partial class LeavesViewModel : BaseViewModel
{
    private readonly ILeaveService _leaveService;
    private readonly IEmployeeService _employeeService;
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// قائمة الإجازات
    /// Leaves list
    /// </summary>
    public ObservableCollection<Leave> Leaves { get; } = new();

    /// <summary>
    /// الإجازات المعلقة
    /// Pending leaves
    /// </summary>
    public ObservableCollection<Leave> PendingLeaves { get; } = new();

    /// <summary>
    /// أنواع الإجازات
    /// Leave types
    /// </summary>
    public ObservableCollection<LeaveType> LeaveTypes { get; } = new();

    /// <summary>
    /// أرصدة الإجازات
    /// Leave balances
    /// </summary>
    public ObservableCollection<LeaveBalance> LeaveBalances { get; } = new();

    /// <summary>
    /// الإجازة المحددة
    /// Selected leave
    /// </summary>
    [ObservableProperty]
    private Leave? _selectedLeave;

    /// <summary>
    /// الموظف المحدد
    /// Selected employee
    /// </summary>
    [ObservableProperty]
    private Employee? _selectedEmployee;

    /// <summary>
    /// السنة المحددة
    /// Selected year
    /// </summary>
    [ObservableProperty]
    private int _selectedYear = DateTime.Now.Year;

    /// <summary>
    /// هل يعرض الإجازات المعلقة فقط؟
    /// Show pending only?
    /// </summary>
    [ObservableProperty]
    private bool _showPendingOnly;

    /// <summary>
    /// الإجازة قيد التحرير
    /// Leave being edited
    /// </summary>
    [ObservableProperty]
    private Leave? _editingLeave;

    /// <summary>
    /// هل في وضع التحرير؟
    /// Is in edit mode?
    /// </summary>
    [ObservableProperty]
    private bool _isEditing;

    /// <summary>
    /// قائمة الموظفين
    /// Employees list
    /// </summary>
    public ObservableCollection<Employee> Employees { get; } = new();

    public LeavesViewModel(
        ILeaveService leaveService,
        IEmployeeService employeeService,
        IAuthenticationService authService)
    {
        _leaveService = leaveService;
        _employeeService = employeeService;
        _authService = authService;
        Title = "إدارة الإجازات";
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            // تحميل أنواع الإجازات
            var types = await _leaveService.GetLeaveTypesAsync();
            LeaveTypes.Clear();
            foreach (var type in types)
            {
                LeaveTypes.Add(type);
            }

            // تحميل الموظفين
            var employees = await _employeeService.GetAllAsync();
            Employees.Clear();
            foreach (var emp in employees)
            {
                Employees.Add(emp);
            }

            // تحميل الإجازات المعلقة
            var pending = await _leaveService.GetPendingLeavesAsync();
            PendingLeaves.Clear();
            foreach (var leave in pending)
            {
                PendingLeaves.Add(leave);
            }

            // تحميل أرصدة الإجازات للموظف الحالي
            if (SelectedEmployee != null)
            {
                await LoadEmployeeLeavesAsync();
            }
        });
    }

    /// <summary>
    /// تحميل إجازات موظف
    /// Load employee leaves
    /// </summary>
    private async Task LoadEmployeeLeavesAsync()
    {
        if (SelectedEmployee == null) return;

        var leaves = await _leaveService.GetByEmployeeAsync(SelectedEmployee.Id);
        Leaves.Clear();
        foreach (var leave in leaves)
        {
            Leaves.Add(leave);
        }

        var balances = await _leaveService.GetBalancesAsync(SelectedEmployee.Id, SelectedYear);
        LeaveBalances.Clear();
        foreach (var balance in balances)
        {
            LeaveBalances.Add(balance);
        }
    }

    /// <summary>
    /// إضافة طلب إجازة جديد
    /// Add new leave request
    /// </summary>
    [RelayCommand]
    private void AddLeave()
    {
        EditingLeave = new Leave
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            Status = Core.Enums.LeaveStatus.Pending
        };
        IsEditing = true;
    }

    /// <summary>
    /// إلغاء التحرير
    /// Cancel editing
    /// </summary>
    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        EditingLeave = null;
    }

    /// <summary>
    /// طلب إجازة جديدة
    /// Request new leave
    /// </summary>
    [RelayCommand]
    private async Task SaveLeaveAsync()
    {
        if (EditingLeave == null) return;

        await ExecuteAsync(async () =>
        {
            EditingLeave.TotalDays = (int)(EditingLeave.EndDate - EditingLeave.StartDate).TotalDays + 1;
            await _leaveService.RequestLeaveAsync(EditingLeave);
            IsEditing = false;
            EditingLeave = null;
            await LoadAsync();
        }, "تم تقديم طلب الإجازة بنجاح");
    }

    /// <summary>
    /// الموافقة على إجازة
    /// Approve leave
    /// </summary>
    [RelayCommand]
    private async Task ApproveLeaveAsync(int leaveId)
    {
        await ExecuteAsync(async () =>
        {
            var approverId = _authService.CurrentUser?.Id ?? 0;
            await _leaveService.ApproveLeaveAsync(leaveId, approverId);
            await LoadAsync();
        }, "تمت الموافقة على الإجازة");
    }

    /// <summary>
    /// رفض إجازة
    /// Reject leave
    /// </summary>
    [RelayCommand]
    private async Task RejectLeaveAsync((int LeaveId, string Reason) args)
    {
        await ExecuteAsync(async () =>
        {
            var approverId = _authService.CurrentUser?.Id ?? 0;
            await _leaveService.RejectLeaveAsync(args.LeaveId, approverId, args.Reason);
            await LoadAsync();
        }, "تم رفض الإجازة");
    }

    /// <summary>
    /// إلغاء إجازة
    /// Cancel leave
    /// </summary>
    [RelayCommand]
    private async Task CancelLeaveAsync(int leaveId)
    {
        await ExecuteAsync(async () =>
        {
            await _leaveService.CancelLeaveAsync(leaveId);
            await LoadAsync();
        }, "تم إلغاء الإجازة");
    }

    partial void OnSelectedEmployeeChanged(Employee? value) => _ = LoadEmployeeLeavesAsync();
    partial void OnSelectedYearChanged(int value) => _ = LoadEmployeeLeavesAsync();
}

