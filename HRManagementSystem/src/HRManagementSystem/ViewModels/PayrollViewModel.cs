// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// PayrollViewModel.cs - ViewModel الرواتب
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الرواتب
/// Payroll ViewModel
/// </summary>
public partial class PayrollViewModel : BaseViewModel
{
    private readonly IPayrollService _payrollService;
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// قائمة الرواتب
    /// Payroll list
    /// </summary>
    public ObservableCollection<Payroll> PayrollList { get; } = new();

    /// <summary>
    /// عناصر الراتب
    /// Salary components
    /// </summary>
    public ObservableCollection<SalaryComponent> SalaryComponents { get; } = new();

    /// <summary>
    /// الراتب المحدد
    /// Selected payroll
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedPayroll))]
    private Payroll? _selectedPayroll;

    /// <summary>
    /// السنة المحددة
    /// Selected year
    /// </summary>
    [ObservableProperty]
    private int _selectedYear = DateTime.Now.Year;

    /// <summary>
    /// الشهر المحدد
    /// Selected month
    /// </summary>
    [ObservableProperty]
    private MonthItem? _selectedMonthItem;

    /// <summary>
    /// رقم الشهر المحدد
    /// Selected month number
    /// </summary>
    public int SelectedMonth => SelectedMonthItem?.Value ?? DateTime.Now.Month;

    /// <summary>
    /// هل يوجد راتب محدد؟
    /// Has selected payroll?
    /// </summary>
    public bool HasSelectedPayroll => SelectedPayroll != null;

    /// <summary>
    /// قائمة السنوات
    /// Years list
    /// </summary>
    public List<int> Years { get; } = Enumerable.Range(2020, DateTime.Now.Year - 2020 + 2).ToList();

    /// <summary>
    /// قائمة الأشهر
    /// Months list
    /// </summary>
    public List<MonthItem> Months { get; } = new()
    {
        new MonthItem(1, "يناير"), new MonthItem(2, "فبراير"), new MonthItem(3, "مارس"),
        new MonthItem(4, "أبريل"), new MonthItem(5, "مايو"), new MonthItem(6, "يونيو"),
        new MonthItem(7, "يوليو"), new MonthItem(8, "أغسطس"), new MonthItem(9, "سبتمبر"),
        new MonthItem(10, "أكتوبر"), new MonthItem(11, "نوفمبر"), new MonthItem(12, "ديسمبر")
    };

    public PayrollViewModel(IPayrollService payrollService, IAuthenticationService authService)
    {
        _payrollService = payrollService;
        _authService = authService;
        Title = "إدارة الرواتب";

        // تعيين الشهر الحالي
        SelectedMonthItem = Months.FirstOrDefault(m => m.Value == DateTime.Now.Month);
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            // تحميل عناصر الراتب
            var components = await _payrollService.GetSalaryComponentsAsync();
            SalaryComponents.Clear();
            foreach (var comp in components)
            {
                SalaryComponents.Add(comp);
            }

            // تحميل الرواتب
            await LoadPayrollAsync();
        });
    }

    /// <summary>
    /// تحميل الرواتب
    /// Load payroll
    /// </summary>
    private async Task LoadPayrollAsync()
    {
        var payrolls = await _payrollService.GetByPeriodAsync(SelectedYear, SelectedMonth);
        PayrollList.Clear();
        foreach (var payroll in payrolls)
        {
            PayrollList.Add(payroll);
        }
    }

    /// <summary>
    /// توليد رواتب الشهر
    /// Generate monthly payroll
    /// </summary>
    [RelayCommand]
    private async Task GeneratePayrollAsync()
    {
        await ExecuteAsync(async () =>
        {
            var createdBy = _authService.CurrentUser?.Id ?? 0;
            await _payrollService.GenerateMonthlyPayrollAsync(SelectedYear, SelectedMonth, createdBy);
            await LoadPayrollAsync();
        }, "تم توليد رواتب الشهر بنجاح");
    }

    /// <summary>
    /// حساب الراتب
    /// Calculate payroll
    /// </summary>
    [RelayCommand]
    private async Task CalculatePayrollAsync(int payrollId)
    {
        await ExecuteAsync(async () =>
        {
            await _payrollService.CalculatePayrollAsync(payrollId);
            await LoadPayrollAsync();
        }, "تم حساب الراتب بنجاح");
    }

    /// <summary>
    /// اعتماد الراتب
    /// Approve payroll
    /// </summary>
    [RelayCommand]
    private async Task ApprovePayrollAsync(int payrollId)
    {
        await ExecuteAsync(async () =>
        {
            var approverId = _authService.CurrentUser?.Id ?? 0;
            await _payrollService.ApprovePayrollAsync(payrollId, approverId);
            await LoadPayrollAsync();
        }, "تم اعتماد الراتب بنجاح");
    }

    /// <summary>
    /// صرف الراتب
    /// Process payment
    /// </summary>
    [RelayCommand]
    private async Task ProcessPaymentAsync(int payrollId)
    {
        await ExecuteAsync(async () =>
        {
            await _payrollService.ProcessPaymentAsync(payrollId);
            await LoadPayrollAsync();
        }, "تم صرف الراتب بنجاح");
    }

    /// <summary>
    /// طباعة كشف الراتب
    /// Print payslip
    /// </summary>
    [RelayCommand]
    private async Task PrintPayslipAsync(int payrollId)
    {
        await ExecuteAsync(async () =>
        {
            var pdf = await _payrollService.GeneratePayslipAsync(payrollId);
            // سيتم حفظ أو عرض الـ PDF
        });
    }

    partial void OnSelectedYearChanged(int value) => _ = LoadPayrollAsync();
    partial void OnSelectedMonthItemChanged(MonthItem? value) => _ = LoadPayrollAsync();
}

/// <summary>
/// عنصر الشهر
/// Month item for ComboBox
/// </summary>
public class MonthItem
{
    public int Value { get; set; }
    public string Name { get; set; }

    public MonthItem(int value, string name)
    {
        Value = value;
        Name = name;
    }
}

