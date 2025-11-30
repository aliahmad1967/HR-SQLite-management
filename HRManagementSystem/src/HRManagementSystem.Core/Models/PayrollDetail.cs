// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// PayrollDetail.cs - نموذج تفاصيل الراتب
// =====================================================

using HRManagementSystem.Core.Enums;

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج تفاصيل الراتب
/// Payroll detail model
/// </summary>
public class PayrollDetail : BaseEntity
{
    /// <summary>
    /// معرف الراتب
    /// Payroll ID
    /// </summary>
    public int PayrollId { get; set; }

    /// <summary>
    /// نوع البند (بدل/خصم)
    /// Item type (allowance/deduction)
    /// </summary>
    public SalaryItemType ItemType { get; set; }

    /// <summary>
    /// اسم البند
    /// Item name
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// المبلغ
    /// Amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// النسبة المئوية
    /// Percentage
    /// </summary>
    public decimal? Percentage { get; set; }

    /// <summary>
    /// هل ثابت؟
    /// Is fixed?
    /// </summary>
    public bool IsFixed { get; set; } = true;

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    // ==================== Navigation Properties ====================

    public Payroll? Payroll { get; set; }

    // ==================== Calculated Properties ====================

    /// <summary>
    /// نوع البند بالعربي
    /// Item type in Arabic
    /// </summary>
    public string ItemTypeDisplayName => ItemType switch
    {
        SalaryItemType.Allowance => "بدل",
        SalaryItemType.Deduction => "خصم",
        _ => "غير محدد"
    };
}

/// <summary>
/// نموذج عنصر الراتب
/// Salary component model
/// </summary>
public class SalaryComponent : ActivatableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public SalaryItemType Type { get; set; }
    public string? CalculationType { get; set; }
    public decimal DefaultAmount { get; set; }
    public decimal DefaultPercentage { get; set; }
    public bool IsTaxable { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// نموذج عنصر راتب الموظف
/// Employee salary component model
/// </summary>
public class EmployeeSalaryComponent : ActivatableEntity
{
    public int EmployeeId { get; set; }
    public int SalaryComponentId { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Employee? Employee { get; set; }
    public SalaryComponent? SalaryComponent { get; set; }
}

