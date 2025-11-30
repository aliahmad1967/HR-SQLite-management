// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Position.cs - نموذج المسمى الوظيفي
// =====================================================

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج المسمى الوظيفي
/// Position model
/// </summary>
public class Position : ActivatableEntity
{
    /// <summary>
    /// المسمى الوظيفي بالعربية
    /// Position title in Arabic
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// المسمى الوظيفي بالإنجليزية
    /// Position title in English
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// معرف القسم
    /// Department ID
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// وصف الوظيفة
    /// Job description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// الحد الأدنى للراتب
    /// Minimum salary
    /// </summary>
    public decimal MinSalary { get; set; }

    /// <summary>
    /// الحد الأعلى للراتب
    /// Maximum salary
    /// </summary>
    public decimal MaxSalary { get; set; }

    // ==================== Navigation Properties ====================

    /// <summary>
    /// القسم
    /// Department
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// الموظفون في هذا المسمى
    /// Employees in this position
    /// </summary>
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

