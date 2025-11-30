// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// MainViewModel.cs - ViewModel الرئيسي
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Services.Interfaces;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الرئيسي - يدير التنقل بين الصفحات
/// Main ViewModel - manages navigation between pages
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// الـ ViewModel الحالي
    /// Current ViewModel
    /// </summary>
    [ObservableProperty]
    private BaseViewModel? _currentViewModel;

    /// <summary>
    /// هل المستخدم مسجل الدخول؟
    /// Is user logged in?
    /// </summary>
    public bool IsLoggedIn => _authService.IsAuthenticated;

    /// <summary>
    /// اسم المستخدم الحالي
    /// Current username
    /// </summary>
    public string CurrentUserName => _authService.CurrentUser?.Username ?? "";

    /// <summary>
    /// دور المستخدم الحالي
    /// Current user role
    /// </summary>
    public string CurrentUserRole => _authService.CurrentUser?.RoleDisplayName ?? "";

    /// <summary>
    /// الصفحة المحددة
    /// Selected page
    /// </summary>
    [ObservableProperty]
    private string _selectedPage = "Dashboard";

    public MainViewModel(IAuthenticationService authService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
        Title = "نظام إدارة الموارد البشرية";
    }

    /// <summary>
    /// التنقل إلى لوحة التحكم
    /// Navigate to dashboard
    /// </summary>
    [RelayCommand]
    private async Task NavigateToDashboardAsync()
    {
        var vm = _serviceProvider.GetService(typeof(DashboardViewModel)) as DashboardViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            await vm.LoadAsync();
            SelectedPage = "Dashboard";
        }
    }

    /// <summary>
    /// التنقل إلى الموظفين
    /// Navigate to employees
    /// </summary>
    [RelayCommand]
    private async Task NavigateToEmployeesAsync()
    {
        var vm = _serviceProvider.GetService(typeof(EmployeesViewModel)) as EmployeesViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            await vm.LoadAsync();
            SelectedPage = "Employees";
        }
    }

    /// <summary>
    /// التنقل إلى الحضور
    /// Navigate to attendance
    /// </summary>
    [RelayCommand]
    private async Task NavigateToAttendanceAsync()
    {
        var vm = _serviceProvider.GetService(typeof(AttendanceViewModel)) as AttendanceViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            await vm.LoadAsync();
            SelectedPage = "Attendance";
        }
    }

    /// <summary>
    /// التنقل إلى الإجازات
    /// Navigate to leaves
    /// </summary>
    [RelayCommand]
    private async Task NavigateToLeavesAsync()
    {
        var vm = _serviceProvider.GetService(typeof(LeavesViewModel)) as LeavesViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            await vm.LoadAsync();
            SelectedPage = "Leaves";
        }
    }

    /// <summary>
    /// التنقل إلى الرواتب
    /// Navigate to payroll
    /// </summary>
    [RelayCommand]
    private async Task NavigateToPayrollAsync()
    {
        var vm = _serviceProvider.GetService(typeof(PayrollViewModel)) as PayrollViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            await vm.LoadAsync();
            SelectedPage = "Payroll";
        }
    }

    /// <summary>
    /// التنقل إلى الأقسام
    /// Navigate to departments
    /// </summary>
    [RelayCommand]
    private async Task NavigateToDepartmentsAsync()
    {
        var vm = _serviceProvider.GetService(typeof(DepartmentsViewModel)) as DepartmentsViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            await vm.LoadAsync();
            SelectedPage = "Departments";
        }
    }

    /// <summary>
    /// التنقل إلى الإعدادات
    /// Navigate to settings
    /// </summary>
    [RelayCommand]
    private void NavigateToSettings()
    {
        var vm = _serviceProvider.GetService(typeof(SettingsViewModel)) as SettingsViewModel;
        if (vm != null)
        {
            CurrentViewModel = vm;
            SelectedPage = "Settings";
        }
    }

    /// <summary>
    /// تسجيل الخروج
    /// Logout
    /// </summary>
    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(CurrentUserName));
        OnPropertyChanged(nameof(CurrentUserRole));
    }
}

