// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Payroll.cs - نموذج الراتب
// =====================================================

using HRManagementSystem.Core.Enums;

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج الراتب الشهري
/// Payroll model
/// </summary>
public class Payroll : BaseEntity
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// شهر الراتب (1-12)
    /// Payroll month
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// سنة الراتب
    /// Payroll year
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// الراتب الأساسي
    /// Basic salary
    /// </summary>
    public decimal BasicSalary { get; set; }

    /// <summary>
    /// إجمالي البدلات
    /// Total allowances
    /// </summary>
    public decimal TotalAllowances { get; set; }

    /// <summary>
    /// إجمالي الخصومات
    /// Total deductions
    /// </summary>
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// مبلغ العمل الإضافي
    /// Overtime amount
    /// </summary>
    public decimal OvertimeAmount { get; set; }

    /// <summary>
    /// ساعات العمل الإضافي
    /// Overtime hours
    /// </summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>
    /// إجمالي الراتب
    /// Gross salary
    /// </summary>
    public decimal GrossSalary { get; set; }

    /// <summary>
    /// صافي الراتب
    /// Net salary
    /// </summary>
    public decimal NetSalary { get; set; }

    /// <summary>
    /// حالة الراتب
    /// Payroll status
    /// </summary>
    public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

    /// <summary>
    /// تاريخ الدفع
    /// Payment date
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// طريقة الدفع
    /// Payment method
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// معرف المعتمد
    /// Approver ID
    /// </summary>
    public int? ApprovedBy { get; set; }

    /// <summary>
    /// تاريخ الاعتماد
    /// Approval date
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    // ==================== Navigation Properties ====================

    public Employee? Employee { get; set; }
    public User? Approver { get; set; }
    public ICollection<PayrollDetail> Details { get; set; } = new List<PayrollDetail>();

    // ==================== Calculated Properties ====================

    /// <summary>
    /// فترة الراتب
    /// Payroll period
    /// </summary>
    public string Period => $"{Year}/{Month:D2}";

    /// <summary>
    /// حالة الراتب بالعربي
    /// Status in Arabic
    /// </summary>
    public string StatusDisplayName => Status switch
    {
        PayrollStatus.Draft => "مسودة",
        PayrollStatus.Approved => "معتمد",
        PayrollStatus.Paid => "مدفوع",
        PayrollStatus.Cancelled => "ملغى",
        _ => "غير محدد"
    };

    /// <summary>
    /// حساب الصافي
    /// Calculate net salary
    /// </summary>
    public void CalculateTotals()
    {
        GrossSalary = BasicSalary + TotalAllowances + OvertimeAmount;
        NetSalary = GrossSalary - TotalDeductions;
    }
}

