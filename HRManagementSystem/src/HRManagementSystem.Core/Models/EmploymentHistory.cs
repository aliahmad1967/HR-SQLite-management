// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// EmploymentHistory.cs - نموذج تاريخ التوظيف
// =====================================================

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج تاريخ التوظيف
/// Employment history model
/// </summary>
public class EmploymentHistory : BaseEntity
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// نوع الإجراء
    /// Action type
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ الإجراء
    /// Action date
    /// </summary>
    public DateTime ActionDate { get; set; }

    /// <summary>
    /// من القسم
    /// From department
    /// </summary>
    public int? FromDepartmentId { get; set; }

    /// <summary>
    /// إلى القسم
    /// To department
    /// </summary>
    public int? ToDepartmentId { get; set; }

    /// <summary>
    /// من المسمى الوظيفي
    /// From position
    /// </summary>
    public int? FromPositionId { get; set; }

    /// <summary>
    /// إلى المسمى الوظيفي
    /// To position
    /// </summary>
    public int? ToPositionId { get; set; }

    /// <summary>
    /// الراتب السابق
    /// Previous salary
    /// </summary>
    public decimal? FromSalary { get; set; }

    /// <summary>
    /// الراتب الجديد
    /// New salary
    /// </summary>
    public decimal? ToSalary { get; set; }

    /// <summary>
    /// السبب
    /// Reason
    /// </summary>
    public string? Reason { get; set; }

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

    // ==================== Navigation Properties ====================

    public Employee? Employee { get; set; }
    public Department? FromDepartment { get; set; }
    public Department? ToDepartment { get; set; }
    public Position? FromPosition { get; set; }
    public Position? ToPosition { get; set; }

    // ==================== Calculated Properties ====================

    /// <summary>
    /// فرق الراتب
    /// Salary difference
    /// </summary>
    public decimal? SalaryDifference => 
        FromSalary.HasValue && ToSalary.HasValue ? 
        ToSalary.Value - FromSalary.Value : null;

    /// <summary>
    /// نسبة تغير الراتب
    /// Salary change percentage
    /// </summary>
    public decimal? SalaryChangePercentage =>
        FromSalary.HasValue && FromSalary > 0 && ToSalary.HasValue ?
        ((ToSalary.Value - FromSalary.Value) / FromSalary.Value) * 100 : null;
}

/// <summary>
/// نموذج سجل النظام
/// Audit log model
/// </summary>
public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public int? RecordId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public User? User { get; set; }
}

/// <summary>
/// نموذج إعدادات النظام
/// System settings model
/// </summary>
public class SystemSetting
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
    public string SettingType { get; set; } = "string";
    public string? Description { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}

