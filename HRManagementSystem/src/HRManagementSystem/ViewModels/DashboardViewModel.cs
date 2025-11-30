// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DashboardViewModel.cs - ViewModel لوحة التحكم
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel لوحة التحكم
/// Dashboard ViewModel
/// </summary>
public partial class DashboardViewModel : BaseViewModel
{
    private readonly IReportService _reportService;
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// إجمالي الموظفين
    /// Total employees
    /// </summary>
    [ObservableProperty]
    private int _totalEmployees;

    /// <summary>
    /// الموظفين النشطين
    /// Active employees
    /// </summary>
    [ObservableProperty]
    private int _activeEmployees;

    /// <summary>
    /// الحاضرين اليوم
    /// Present today
    /// </summary>
    [ObservableProperty]
    private int _presentToday;

    /// <summary>
    /// الغائبين اليوم
    /// Absent today
    /// </summary>
    [ObservableProperty]
    private int _absentToday;

    /// <summary>
    /// في إجازة اليوم
    /// On leave today
    /// </summary>
    [ObservableProperty]
    private int _onLeaveToday;

    /// <summary>
    /// طلبات الإجازة المعلقة
    /// Pending leave requests
    /// </summary>
    [ObservableProperty]
    private int _pendingLeaveRequests;

    /// <summary>
    /// إجمالي رواتب الشهر
    /// Total payroll this month
    /// </summary>
    [ObservableProperty]
    private decimal _totalPayrollThisMonth;

    /// <summary>
    /// المستندات القريبة من الانتهاء
    /// Expiring documents
    /// </summary>
    [ObservableProperty]
    private int _expiringDocuments;

    /// <summary>
    /// إحصائيات الأقسام
    /// Department statistics
    /// </summary>
    public ObservableCollection<DepartmentStats> DepartmentStats { get; } = new();

    /// <summary>
    /// إحصائيات الحضور الشهري
    /// Monthly attendance statistics
    /// </summary>
    public ObservableCollection<MonthlyAttendanceStats> MonthlyAttendance { get; } = new();

    /// <summary>
    /// اسم المستخدم الحالي
    /// Current username
    /// </summary>
    public string CurrentUserName => _authService.CurrentUser?.Username ?? "زائر";

    /// <summary>
    /// دور المستخدم الحالي
    /// Current user role
    /// </summary>
    public string CurrentUserRole => _authService.CurrentUser?.RoleDisplayName ?? "";

    public DashboardViewModel(IReportService reportService, IAuthenticationService authService)
    {
        _reportService = reportService;
        _authService = authService;
        Title = "لوحة التحكم";
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var stats = await _reportService.GetDashboardStatsAsync();

            TotalEmployees = stats.TotalEmployees;
            ActiveEmployees = stats.ActiveEmployees;
            PresentToday = stats.PresentToday;
            AbsentToday = stats.AbsentToday;
            OnLeaveToday = stats.OnLeaveToday;
            PendingLeaveRequests = stats.PendingLeaveRequests;
            TotalPayrollThisMonth = stats.TotalPayrollThisMonth;
            ExpiringDocuments = stats.ExpiringDocuments;

            DepartmentStats.Clear();
            foreach (var dept in stats.DepartmentStats)
            {
                DepartmentStats.Add(dept);
            }

            MonthlyAttendance.Clear();
            foreach (var month in stats.MonthlyAttendance)
            {
                MonthlyAttendance.Add(month);
            }
        });
    }

    /// <summary>
    /// تسجيل الخروج
    /// Logout
    /// </summary>
    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        // سيتم التعامل مع التنقل في الـ View
    }
}

