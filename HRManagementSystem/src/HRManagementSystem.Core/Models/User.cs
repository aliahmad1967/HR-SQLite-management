// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// User.cs - نموذج المستخدم
// =====================================================

using HRManagementSystem.Core.Enums;

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج المستخدم
/// User model
/// </summary>
public class User : ActivatableEntity
{
    /// <summary>
    /// اسم المستخدم
    /// Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// كلمة المرور المشفرة
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// البريد الإلكتروني
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// معرف الموظف المرتبط
    /// Linked employee ID
    /// </summary>
    public int? EmployeeId { get; set; }

    /// <summary>
    /// دور المستخدم
    /// User role
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Viewer;

    /// <summary>
    /// آخر تسجيل دخول
    /// Last login date
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// محاولات الدخول الفاشلة
    /// Failed login attempts
    /// </summary>
    public int FailedLoginAttempts { get; set; }

    /// <summary>
    /// مقفل حتى
    /// Locked until
    /// </summary>
    public DateTime? LockedUntil { get; set; }

    // ==================== Navigation Properties ====================

    /// <summary>
    /// الموظف المرتبط
    /// Linked employee
    /// </summary>
    public Employee? Employee { get; set; }

    // ==================== Calculated Properties ====================

    /// <summary>
    /// هل الحساب مقفل؟
    /// Is account locked?
    /// </summary>
    public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.Now;

    /// <summary>
    /// اسم العرض
    /// Display name
    /// </summary>
    public string DisplayName => Employee?.FullName ?? Username;

    /// <summary>
    /// اسم الدور بالعربي
    /// Role name in Arabic
    /// </summary>
    public string RoleDisplayName => Role switch
    {
        UserRole.Admin => "مدير النظام",
        UserRole.HRManager => "مدير الموارد البشرية",
        UserRole.Viewer => "مستخدم",
        _ => "غير محدد"
    };
}

