// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Department.cs - نموذج القسم
// =====================================================

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج القسم
/// Department model
/// </summary>
public class Department : ActivatableEntity
{
    /// <summary>
    /// اسم القسم بالعربية
    /// Department name in Arabic
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// اسم القسم بالإنجليزية
    /// Department name in English
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// وصف القسم
    /// Department description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// معرف مدير القسم
    /// Manager ID
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// معرف القسم الأب (للهيكل الهرمي)
    /// Parent department ID (for hierarchy)
    /// </summary>
    public int? ParentDepartmentId { get; set; }

    // ==================== Navigation Properties ====================

    /// <summary>
    /// مدير القسم
    /// Department manager
    /// </summary>
    public Employee? Manager { get; set; }

    /// <summary>
    /// القسم الأب
    /// Parent department
    /// </summary>
    public Department? ParentDepartment { get; set; }

    /// <summary>
    /// الأقسام الفرعية
    /// Child departments
    /// </summary>
    public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();

    /// <summary>
    /// موظفو القسم
    /// Department employees
    /// </summary>
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    /// <summary>
    /// المسميات الوظيفية في القسم
    /// Positions in the department
    /// </summary>
    public ICollection<Position> Positions { get; set; } = new List<Position>();

    // ==================== Calculated Properties ====================

    /// <summary>
    /// عدد الموظفين
    /// Employee count
    /// </summary>
    public int EmployeeCount => Employees?.Count(e => e.IsActive) ?? 0;
}

