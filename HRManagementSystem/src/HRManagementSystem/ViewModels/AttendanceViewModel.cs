// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// AttendanceViewModel.cs - ViewModel الحضور
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الحضور
/// Attendance ViewModel
/// </summary>
public partial class AttendanceViewModel : BaseViewModel
{
    private readonly IAttendanceService _attendanceService;
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    /// <summary>
    /// سجلات الحضور
    /// Attendance records
    /// </summary>
    public ObservableCollection<Attendance> AttendanceRecords { get; } = new();

    /// <summary>
    /// قائمة الموظفين
    /// Employees list
    /// </summary>
    public ObservableCollection<Employee> Employees { get; } = new();

    /// <summary>
    /// قائمة الأقسام
    /// Departments list
    /// </summary>
    public ObservableCollection<Department> Departments { get; } = new();

    /// <summary>
    /// التاريخ المحدد
    /// Selected date
    /// </summary>
    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    /// <summary>
    /// القسم المحدد
    /// Selected department
    /// </summary>
    [ObservableProperty]
    private Department? _selectedDepartment;

    /// <summary>
    /// الموظف المحدد
    /// Selected employee
    /// </summary>
    [ObservableProperty]
    private Employee? _selectedEmployee;

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(-30);

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    [ObservableProperty]
    private DateTime _endDate = DateTime.Today;

    public AttendanceViewModel(
        IAttendanceService attendanceService,
        IEmployeeService employeeService,
        IDepartmentService departmentService)
    {
        _attendanceService = attendanceService;
        _employeeService = employeeService;
        _departmentService = departmentService;
        Title = "إدارة الحضور والانصراف";
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            // تحميل الأقسام
            var departments = await _departmentService.GetActiveDepartmentsAsync();
            Departments.Clear();
            foreach (var dept in departments)
            {
                Departments.Add(dept);
            }

            // تحميل الموظفين
            var employees = await _employeeService.GetAllAsync();
            Employees.Clear();
            foreach (var emp in employees)
            {
                Employees.Add(emp);
            }

            // تحميل سجلات الحضور
            await LoadAttendanceAsync();
        });
    }

    /// <summary>
    /// تحميل سجلات الحضور
    /// Load attendance records
    /// </summary>
    private async Task LoadAttendanceAsync()
    {
        AttendanceRecords.Clear();

        if (SelectedEmployee != null)
        {
            var records = await _attendanceService.GetByDateRangeAsync(
                SelectedEmployee.Id, StartDate, EndDate);
            foreach (var record in records)
            {
                AttendanceRecords.Add(record);
            }
        }
        else if (SelectedDepartment != null)
        {
            var records = await _attendanceService.GetDepartmentAttendanceAsync(
                SelectedDepartment.Id, SelectedDate);
            foreach (var record in records)
            {
                AttendanceRecords.Add(record);
            }
        }
    }

    /// <summary>
    /// تسجيل الحضور
    /// Check in
    /// </summary>
    [RelayCommand]
    private async Task CheckInAsync(int employeeId)
    {
        await ExecuteAsync(async () =>
        {
            await _attendanceService.CheckInAsync(employeeId);
            await LoadAttendanceAsync();
        }, "تم تسجيل الحضور بنجاح");
    }

    /// <summary>
    /// تسجيل الانصراف
    /// Check out
    /// </summary>
    [RelayCommand]
    private async Task CheckOutAsync(int employeeId)
    {
        await ExecuteAsync(async () =>
        {
            await _attendanceService.CheckOutAsync(employeeId);
            await LoadAttendanceAsync();
        }, "تم تسجيل الانصراف بنجاح");
    }

    /// <summary>
    /// عند تغيير التاريخ
    /// On date changed
    /// </summary>
    partial void OnSelectedDateChanged(DateTime value) => _ = LoadAttendanceAsync();
    partial void OnSelectedDepartmentChanged(Department? value) => _ = LoadAttendanceAsync();
    partial void OnSelectedEmployeeChanged(Employee? value) => _ = LoadAttendanceAsync();
}

