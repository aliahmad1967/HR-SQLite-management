// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DepartmentService.cs - خدمة الأقسام
// =====================================================

using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Serilog;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة الأقسام
/// Department service
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = Log.ForContext<DepartmentService>();
    }

    /// <summary>
    /// جلب جميع الأقسام
    /// Get all departments
    /// </summary>
    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _unitOfWork.Departments.GetAllAsync();
    }

    /// <summary>
    /// جلب الأقسام النشطة
    /// Get active departments
    /// </summary>
    public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
    {
        return await _unitOfWork.Departments.GetActiveDepartmentsAsync();
    }

    /// <summary>
    /// جلب قسم بالمعرف
    /// Get department by ID
    /// </summary>
    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Departments.GetByIdAsync(id);
    }

    /// <summary>
    /// جلب قسم مع موظفيه
    /// Get department with employees
    /// </summary>
    public async Task<Department?> GetWithEmployeesAsync(int id)
    {
        return await _unitOfWork.Departments.GetWithEmployeesAsync(id);
    }

    /// <summary>
    /// إنشاء قسم جديد
    /// Create new department
    /// </summary>
    public async Task<int> CreateAsync(Department department)
    {
        try
        {
            // التحقق من عدم تكرار الاسم
            var existing = await _unitOfWork.Departments.GetByNameAsync(department.Name);
            if (existing != null)
            {
                _logger.Warning("محاولة إضافة قسم باسم موجود: {Name}", department.Name);
                throw new InvalidOperationException("اسم القسم موجود مسبقاً");
            }

            department.IsActive = true;
            var id = await _unitOfWork.Departments.AddAsync(department);
            _logger.Information("تم إنشاء قسم جديد: {DepartmentId} - {Name}", id, department.Name);

            return id;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء إنشاء قسم جديد");
            throw;
        }
    }

    /// <summary>
    /// تحديث قسم
    /// Update department
    /// </summary>
    public async Task<bool> UpdateAsync(Department department)
    {
        try
        {
            var result = await _unitOfWork.Departments.UpdateAsync(department);
            if (result)
                _logger.Information("تم تحديث القسم: {DepartmentId}", department.Id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تحديث القسم: {DepartmentId}", department.Id);
            throw;
        }
    }

    /// <summary>
    /// حذف قسم
    /// Delete department
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            // التحقق من عدم وجود موظفين
            var department = await _unitOfWork.Departments.GetWithEmployeesAsync(id);
            if (department?.Employees?.Any() == true)
            {
                _logger.Warning("لا يمكن حذف قسم يحتوي على موظفين: {DepartmentId}", id);
                throw new InvalidOperationException("لا يمكن حذف قسم يحتوي على موظفين");
            }

            var result = await _unitOfWork.Departments.DeleteAsync(id);
            if (result)
                _logger.Information("تم حذف القسم: {DepartmentId}", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء حذف القسم: {DepartmentId}", id);
            throw;
        }
    }
}

