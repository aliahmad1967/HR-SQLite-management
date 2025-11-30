// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// EmployeeService.cs - خدمة الموظفين
// =====================================================

using HRManagementSystem.Core.Enums;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Serilog;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة الموظفين
/// Employee service
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = Log.ForContext<EmployeeService>();
    }

    /// <summary>
    /// جلب جميع الموظفين
    /// Get all employees
    /// </summary>
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _unitOfWork.Employees.GetActiveEmployeesAsync();
    }

    /// <summary>
    /// جلب موظف بالمعرف
    /// Get employee by ID
    /// </summary>
    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Employees.GetByIdAsync(id);
    }

    /// <summary>
    /// جلب موظف مع التفاصيل
    /// Get employee with details
    /// </summary>
    public async Task<Employee?> GetWithDetailsAsync(int id)
    {
        return await _unitOfWork.Employees.GetWithDetailsAsync(id);
    }

    /// <summary>
    /// البحث عن موظفين
    /// Search employees
    /// </summary>
    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        return await _unitOfWork.Employees.SearchAsync(searchTerm);
    }

    /// <summary>
    /// جلب موظفي قسم معين
    /// Get employees by department
    /// </summary>
    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
    {
        return await _unitOfWork.Employees.GetByDepartmentAsync(departmentId);
    }

    /// <summary>
    /// إنشاء موظف جديد
    /// Create new employee
    /// </summary>
    public async Task<int> CreateAsync(Employee employee)
    {
        try
        {
            // التحقق من عدم تكرار رقم الهوية (فقط إذا كان موجوداً)
            if (!string.IsNullOrWhiteSpace(employee.NationalId))
            {
                var existingByNationalId = await _unitOfWork.Employees.GetByNationalIdAsync(employee.NationalId);
                if (existingByNationalId != null)
                {
                    _logger.Warning("محاولة إضافة موظف برقم هوية موجود: {NationalId}", employee.NationalId);
                    throw new InvalidOperationException("رقم الهوية مسجل مسبقاً");
                }
            }

            employee.IsActive = true;
            employee.EmploymentStatus = EmploymentStatus.Active;
            employee.CreatedAt = DateTime.Now;

            var id = await _unitOfWork.Employees.AddAsync(employee);
            _logger.Information("تم إنشاء موظف جديد: {EmployeeId} - {EmployeeName}", id, employee.FullNameAr);

            return id;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء إنشاء موظف جديد");
            throw;
        }
    }

    /// <summary>
    /// تحديث بيانات موظف
    /// Update employee
    /// </summary>
    public async Task<bool> UpdateAsync(Employee employee)
    {
        try
        {
            employee.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.Employees.UpdateAsync(employee);
            
            if (result)
                _logger.Information("تم تحديث بيانات الموظف: {EmployeeId}", employee.Id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تحديث بيانات الموظف: {EmployeeId}", employee.Id);
            throw;
        }
    }

    /// <summary>
    /// حذف موظف (حذف ناعم)
    /// Delete employee (soft delete)
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return false;

            employee.IsActive = false;
            employee.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.Employees.UpdateAsync(employee);

            if (result)
                _logger.Information("تم حذف الموظف: {EmployeeId}", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء حذف الموظف: {EmployeeId}", id);
            throw;
        }
    }

    /// <summary>
    /// إنهاء خدمة موظف
    /// Terminate employee
    /// </summary>
    public async Task<bool> TerminateAsync(int id, string reason)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return false;

            employee.EmploymentStatus = EmploymentStatus.Terminated;
            employee.Notes = $"{employee.Notes}\nسبب إنهاء الخدمة: {reason} - تاريخ الإنهاء: {DateTime.Now:yyyy-MM-dd}";
            employee.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.Employees.UpdateAsync(employee);

            if (result)
                _logger.Information("تم إنهاء خدمة الموظف: {EmployeeId} - السبب: {Reason}", id, reason);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء إنهاء خدمة الموظف: {EmployeeId}", id);
            throw;
        }
    }
}

