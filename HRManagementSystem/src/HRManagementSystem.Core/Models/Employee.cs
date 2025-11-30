// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Employee.cs - نموذج الموظف
// =====================================================

using HRManagementSystem.Core.Enums;

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج الموظف
/// Employee model
/// </summary>
public class Employee : ActivatableEntity
{
    // ==================== معلومات الهوية ====================
    
    /// <summary>
    /// رقم الموظف (تلقائي)
    /// Employee number (auto-generated)
    /// </summary>
    public string EmployeeNumber { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الأول
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الثاني
    /// Second name
    /// </summary>
    public string? SecondName { get; set; }

    /// <summary>
    /// الاسم الثالث
    /// Third name
    /// </summary>
    public string? ThirdName { get; set; }

    /// <summary>
    /// اسم العائلة
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الكامل بالعربي
    /// Full name in Arabic
    /// </summary>
    public string? FullNameAr { get; set; }

    /// <summary>
    /// الاسم الكامل بالإنجليزي
    /// Full name in English
    /// </summary>
    public string? FullNameEn { get; set; }

    /// <summary>
    /// رقم الهوية الوطنية
    /// National ID
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// الجنسية
    /// Nationality
    /// </summary>
    public string? Nationality { get; set; }

    /// <summary>
    /// الجنس
    /// Gender
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// تاريخ الميلاد
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// مكان الميلاد
    /// Place of birth
    /// </summary>
    public string? PlaceOfBirth { get; set; }

    /// <summary>
    /// الحالة الاجتماعية
    /// Marital status
    /// </summary>
    public MaritalStatus? MaritalStatus { get; set; }

    // ==================== معلومات الاتصال ====================

    /// <summary>
    /// البريد الإلكتروني
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// رقم الهاتف
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// رقم الجوال
    /// Mobile number
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// العنوان
    /// Address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// المدينة
    /// City
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// الرمز البريدي
    /// Postal code
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// اسم جهة الاتصال للطوارئ
    /// Emergency contact name
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// رقم جهة الاتصال للطوارئ
    /// Emergency contact phone
    /// </summary>
    public string? EmergencyContactPhone { get; set; }

    // ==================== معلومات الوظيفة ====================

    /// <summary>
    /// معرف القسم
    /// Department ID
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// معرف المسمى الوظيفي
    /// Position ID
    /// </summary>
    public int? PositionId { get; set; }

    /// <summary>
    /// معرف المدير المباشر
    /// Direct manager ID
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// تاريخ التعيين
    /// Hire date
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// نوع العقد
    /// Contract type
    /// </summary>
    public ContractType? ContractType { get; set; }

    /// <summary>
    /// تاريخ انتهاء العقد
    /// Contract end date
    /// </summary>
    public DateTime? ContractEndDate { get; set; }

    /// <summary>
    /// حالة التوظيف
    /// Employment status
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;

    // ==================== معلومات مالية ====================

    /// <summary>
    /// الراتب الأساسي
    /// Basic salary
    /// </summary>
    public decimal BasicSalary { get; set; }

    /// <summary>
    /// اسم البنك
    /// Bank name
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// رقم الحساب البنكي
    /// Bank account number
    /// </summary>
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// رقم الآيبان
    /// IBAN
    /// </summary>
    public string? IBAN { get; set; }

    // ==================== معلومات أخرى ====================

    /// <summary>
    /// مسار صورة الموظف
    /// Profile photo path
    /// </summary>
    public string? ProfilePhoto { get; set; }

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    // ==================== Navigation Properties ====================

    public Department? Department { get; set; }
    public Position? Position { get; set; }
    public Employee? Manager { get; set; }
    public User? User { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    public ICollection<Leave> Leaves { get; set; } = new List<Leave>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    public ICollection<Payroll> PayrollRecords { get; set; } = new List<Payroll>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();

    // ==================== Calculated Properties ====================

    /// <summary>
    /// الاسم الكامل
    /// Full name
    /// </summary>
    public string FullName => string.Join(" ", new[] { FirstName, SecondName, ThirdName, LastName }
        .Where(s => !string.IsNullOrWhiteSpace(s)));

    /// <summary>
    /// سنوات الخدمة
    /// Years of service
    /// </summary>
    public double YearsOfService => (DateTime.Now - HireDate).TotalDays / 365.25;

    /// <summary>
    /// العمر
    /// Age
    /// </summary>
    public int? Age => DateOfBirth.HasValue ?
        (int)((DateTime.Now - DateOfBirth.Value).TotalDays / 365.25) : null;
}

