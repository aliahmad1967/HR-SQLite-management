// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Attendance.cs - نموذج الحضور
// =====================================================

using HRManagementSystem.Core.Enums;

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج الحضور
/// Attendance model
/// </summary>
public class Attendance : BaseEntity
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// تاريخ الحضور
    /// Attendance date
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// وقت الحضور
    /// Check-in time
    /// </summary>
    public TimeSpan? CheckInTime { get; set; }

    /// <summary>
    /// وقت الانصراف
    /// Check-out time
    /// </summary>
    public TimeSpan? CheckOutTime { get; set; }

    /// <summary>
    /// حالة الحضور
    /// Attendance status
    /// </summary>
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    /// <summary>
    /// ساعات العمل
    /// Working hours
    /// </summary>
    public decimal? WorkingHours { get; set; }

    /// <summary>
    /// ساعات إضافية
    /// Overtime hours
    /// </summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// هل إدخال يدوي؟
    /// Is manual entry?
    /// </summary>
    public bool IsManualEntry { get; set; }

    // ==================== Navigation Properties ====================

    /// <summary>
    /// الموظف
    /// Employee
    /// </summary>
    public Employee? Employee { get; set; }

    // ==================== Calculated Properties ====================

    /// <summary>
    /// صيغة وقت الحضور
    /// Formatted check-in time
    /// </summary>
    public string CheckInTimeFormatted => CheckInTime?.ToString(@"hh\:mm") ?? "--:--";

    /// <summary>
    /// صيغة وقت الانصراف
    /// Formatted check-out time
    /// </summary>
    public string CheckOutTimeFormatted => CheckOutTime?.ToString(@"hh\:mm") ?? "--:--";

    /// <summary>
    /// حالة الحضور بالعربي
    /// Status in Arabic
    /// </summary>
    public string StatusDisplayName => Status switch
    {
        AttendanceStatus.Present => "حاضر",
        AttendanceStatus.Absent => "غائب",
        AttendanceStatus.Late => "متأخر",
        AttendanceStatus.OnLeave => "إجازة",
        AttendanceStatus.BusinessTrip => "مهمة عمل",
        _ => "غير محدد"
    };
}

