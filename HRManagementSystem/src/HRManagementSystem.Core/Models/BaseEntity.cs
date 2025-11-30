// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// BaseEntity.cs - الكيان الأساسي
// =====================================================

namespace HRManagementSystem.Core.Models;

/// <summary>
/// الكيان الأساسي لجميع الجداول
/// Base entity for all database tables
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// المعرف الفريد
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// تاريخ الإنشاء
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// تاريخ التحديث
    /// Update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// معرف المستخدم المنشئ
    /// Creator user ID
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// معرف المستخدم المحدث
    /// Updater user ID
    /// </summary>
    public int? UpdatedBy { get; set; }
}

/// <summary>
/// كيان قابل للتفعيل/التعطيل
/// Entity that can be activated/deactivated
/// </summary>
public abstract class ActivatableEntity : BaseEntity
{
    /// <summary>
    /// حالة النشاط
    /// Active status
    /// </summary>
    public bool IsActive { get; set; } = true;
}

