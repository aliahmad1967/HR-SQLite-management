// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DepartmentsViewModel.cs - ViewModel الأقسام
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الأقسام
/// Departments ViewModel
/// </summary>
public partial class DepartmentsViewModel : BaseViewModel
{
    private readonly IDepartmentService _departmentService;

    /// <summary>
    /// قائمة الأقسام
    /// Departments list
    /// </summary>
    public ObservableCollection<Department> Departments { get; } = new();

    /// <summary>
    /// القسم المحدد
    /// Selected department
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedDepartment))]
    private Department? _selectedDepartment;

    /// <summary>
    /// القسم قيد التحرير
    /// Department being edited
    /// </summary>
    [ObservableProperty]
    private Department? _editingDepartment;

    /// <summary>
    /// هل يوجد قسم محدد؟
    /// Has selected department?
    /// </summary>
    public bool HasSelectedDepartment => SelectedDepartment != null;

    /// <summary>
    /// هل في وضع التحرير؟
    /// Is in edit mode?
    /// </summary>
    [ObservableProperty]
    private bool _isEditing;

    public DepartmentsViewModel(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
        Title = "إدارة الأقسام";
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var departments = await _departmentService.GetAllAsync();
            Departments.Clear();
            foreach (var dept in departments)
            {
                Departments.Add(dept);
            }
        });
    }

    /// <summary>
    /// إضافة قسم جديد
    /// Add new department
    /// </summary>
    [RelayCommand]
    private void AddDepartment()
    {
        EditingDepartment = new Department { IsActive = true };
        IsEditing = true;
    }

    /// <summary>
    /// تعديل قسم
    /// Edit department
    /// </summary>
    [RelayCommand]
    private async Task EditDepartmentAsync()
    {
        if (SelectedDepartment == null) return;

        var department = await _departmentService.GetWithEmployeesAsync(SelectedDepartment.Id);
        EditingDepartment = department;
        IsEditing = true;
    }

    /// <summary>
    /// حفظ القسم
    /// Save department
    /// </summary>
    [RelayCommand]
    private async Task SaveDepartmentAsync()
    {
        if (EditingDepartment == null) return;

        await ExecuteAsync(async () =>
        {
            if (EditingDepartment.Id == 0)
            {
                await _departmentService.CreateAsync(EditingDepartment);
            }
            else
            {
                await _departmentService.UpdateAsync(EditingDepartment);
            }

            IsEditing = false;
            EditingDepartment = null;
            await LoadAsync();
        }, "تم حفظ القسم بنجاح");
    }

    /// <summary>
    /// إلغاء التحرير
    /// Cancel editing
    /// </summary>
    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        EditingDepartment = null;
    }

    /// <summary>
    /// حذف قسم
    /// Delete department
    /// </summary>
    [RelayCommand]
    private async Task DeleteDepartmentAsync()
    {
        if (SelectedDepartment == null) return;

        await ExecuteAsync(async () =>
        {
            await _departmentService.DeleteAsync(SelectedDepartment.Id);
            Departments.Remove(SelectedDepartment);
            SelectedDepartment = null;
        }, "تم حذف القسم بنجاح");
    }
}

