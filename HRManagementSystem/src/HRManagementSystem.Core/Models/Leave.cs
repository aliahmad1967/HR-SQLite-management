// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Leave.cs - نموذج الإجازة
// =====================================================

using HRManagementSystem.Core.Enums;

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج الإجازة
/// Leave model
/// </summary>
public class Leave : BaseEntity
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// معرف نوع الإجازة
    /// Leave type ID
    /// </summary>
    public int LeaveTypeId { get; set; }

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// إجمالي الأيام
    /// Total days
    /// </summary>
    public int TotalDays { get; set; }

    /// <summary>
    /// السبب
    /// Reason
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// حالة الإجازة
    /// Leave status
    /// </summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

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

    /// <summary>
    /// سبب الرفض
    /// Rejection reason
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// مسار المرفق
    /// Attachment path
    /// </summary>
    public string? AttachmentPath { get; set; }

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    // ==================== Navigation Properties ====================

    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
    public User? Approver { get; set; }

    // ==================== Calculated Properties ====================

    /// <summary>
    /// حالة الإجازة بالعربي
    /// Status in Arabic
    /// </summary>
    public string StatusDisplayName => Status switch
    {
        LeaveStatus.Pending => "قيد الانتظار",
        LeaveStatus.Approved => "موافق عليها",
        LeaveStatus.Rejected => "مرفوضة",
        LeaveStatus.Cancelled => "ملغاة",
        _ => "غير محدد"
    };

    /// <summary>
    /// لون الحالة
    /// Status color
    /// </summary>
    public string StatusColor => Status switch
    {
        LeaveStatus.Pending => "#FFA500",
        LeaveStatus.Approved => "#4CAF50",
        LeaveStatus.Rejected => "#F44336",
        LeaveStatus.Cancelled => "#9E9E9E",
        _ => "#000000"
    };
}

/// <summary>
/// نموذج نوع الإجازة
/// Leave type model
/// </summary>
public class LeaveType : ActivatableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public int DefaultDays { get; set; }
    public bool IsPaid { get; set; } = true;
    public bool RequiresApproval { get; set; } = true;
    public string Color { get; set; } = "#4CAF50";
    public ICollection<Leave> Leaves { get; set; } = new List<Leave>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}

/// <summary>
/// نموذج رصيد الإجازة
/// Leave balance model
/// </summary>
public class LeaveBalance : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal TotalDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal RemainingDays { get; set; }
    public decimal CarriedOverDays { get; set; }
    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
}

