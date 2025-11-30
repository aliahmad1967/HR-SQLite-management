// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Enums.cs - التعدادات
// =====================================================

namespace HRManagementSystem.Core.Enums;

/// <summary>
/// الجنس
/// Gender
/// </summary>
public enum Gender
{
    [Display("ذكر")]
    Male,
    [Display("أنثى")]
    Female
}

/// <summary>
/// الحالة الاجتماعية
/// Marital Status
/// </summary>
public enum MaritalStatus
{
    [Display("أعزب")]
    Single,
    [Display("متزوج")]
    Married,
    [Display("مطلق")]
    Divorced,
    [Display("أرمل")]
    Widowed
}

/// <summary>
/// نوع العقد
/// Contract Type
/// </summary>
public enum ContractType
{
    [Display("دائم")]
    Permanent,
    [Display("مؤقت")]
    Temporary,
    [Display("متعاقد")]
    Contract,
    [Display("تدريب")]
    Internship,
    [Display("دوام جزئي")]
    PartTime
}

/// <summary>
/// حالة التوظيف
/// Employment Status
/// </summary>
public enum EmploymentStatus
{
    [Display("نشط")]
    Active,
    [Display("في إجازة")]
    OnLeave,
    [Display("موقوف")]
    Suspended,
    [Display("مستقيل")]
    Resigned,
    [Display("منتهي")]
    Terminated
}

/// <summary>
/// حالة الحضور
/// Attendance Status
/// </summary>
public enum AttendanceStatus
{
    [Display("حاضر")]
    Present,
    [Display("غائب")]
    Absent,
    [Display("متأخر")]
    Late,
    [Display("إجازة")]
    OnLeave,
    [Display("مهمة عمل")]
    BusinessTrip
}

/// <summary>
/// حالة الإجازة
/// Leave Status
/// </summary>
public enum LeaveStatus
{
    [Display("قيد الانتظار")]
    Pending,
    [Display("موافق عليها")]
    Approved,
    [Display("مرفوضة")]
    Rejected,
    [Display("ملغاة")]
    Cancelled
}

/// <summary>
/// حالة الراتب
/// Payroll Status
/// </summary>
public enum PayrollStatus
{
    [Display("مسودة")]
    Draft,
    [Display("معتمد")]
    Approved,
    [Display("مدفوع")]
    Paid,
    [Display("ملغى")]
    Cancelled,
    [Display("تمت المعالجة")]
    Processed
}

/// <summary>
/// نوع عنصر الراتب
/// Salary Item Type
/// </summary>
public enum SalaryItemType
{
    [Display("بدل")]
    Allowance,
    [Display("خصم")]
    Deduction
}

/// <summary>
/// دور المستخدم
/// User Role
/// </summary>
public enum UserRole
{
    [Display("مدير النظام")]
    Admin,
    [Display("مدير الموارد البشرية")]
    HRManager,
    [Display("مستخدم")]
    Viewer
}

/// <summary>
/// سمة عرض للتعداد
/// Display attribute for enums
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DisplayAttribute : Attribute
{
    public string Name { get; }
    public DisplayAttribute(string name) => Name = name;
}

