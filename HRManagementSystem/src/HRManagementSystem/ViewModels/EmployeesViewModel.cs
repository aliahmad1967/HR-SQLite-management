// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// EmployeesViewModel.cs - ViewModel الموظفين
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الموظفين
/// Employees ViewModel
/// </summary>
public partial class EmployeesViewModel : BaseViewModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    /// <summary>
    /// قائمة الموظفين
    /// Employees list
    /// </summary>
    public ObservableCollection<Employee> Employees { get; } = new();

    /// <summary>
    /// قائمة الأقسام
    /// Departments list
    /// </summary>
    public ObservableCollection<Department> Departments { get; } = new();

    /// <summary>
    /// الموظف المحدد
    /// Selected employee
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedEmployee))]
    private Employee? _selectedEmployee;

    /// <summary>
    /// نص البحث
    /// Search text
    /// </summary>
    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>
    /// القسم المحدد للفلترة
    /// Selected department for filtering
    /// </summary>
    [ObservableProperty]
    private Department? _selectedDepartment;

    /// <summary>
    /// هل يوجد موظف محدد؟
    /// Has selected employee?
    /// </summary>
    public bool HasSelectedEmployee => SelectedEmployee != null;

    /// <summary>
    /// الموظف قيد التحرير
    /// Employee being edited
    /// </summary>
    [ObservableProperty]
    private Employee? _editingEmployee;

    /// <summary>
    /// هل في وضع التحرير؟
    /// Is in edit mode?
    /// </summary>
    [ObservableProperty]
    private bool _isEditing;

    /// <summary>
    /// صورة الموظف الحالية
    /// Current employee image
    /// </summary>
    [ObservableProperty]
    private BitmapImage? _employeeImage;

    /// <summary>
    /// مسار مجلد الصور
    /// Images folder path
    /// </summary>
    private static string ImagesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images");

    public EmployeesViewModel(IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        Title = "إدارة الموظفين";
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            // تحميل الأقسام
            var departments = await _departmentService.GetActiveDepartmentsAsync();
            Departments.Clear();
            Departments.Add(new Department { Id = 0, Name = "جميع الأقسام" });
            foreach (var dept in departments)
            {
                Departments.Add(dept);
            }

            // تحميل الموظفين
            await LoadEmployeesAsync();
        });
    }

    /// <summary>
    /// تحميل الموظفين
    /// Load employees
    /// </summary>
    private async Task LoadEmployeesAsync()
    {
        IEnumerable<Employee> employees;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            employees = await _employeeService.SearchAsync(SearchText);
        }
        else if (SelectedDepartment != null && SelectedDepartment.Id > 0)
        {
            employees = await _employeeService.GetByDepartmentAsync(SelectedDepartment.Id);
        }
        else
        {
            employees = await _employeeService.GetAllAsync();
        }

        Employees.Clear();
        foreach (var emp in employees)
        {
            Employees.Add(emp);
        }
    }

    /// <summary>
    /// البحث
    /// Search
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        await ExecuteAsync(LoadEmployeesAsync);
    }

    /// <summary>
    /// عند تغيير القسم
    /// On department changed
    /// </summary>
    partial void OnSelectedDepartmentChanged(Department? value)
    {
        _ = LoadEmployeesAsync();
    }

    /// <summary>
    /// إضافة موظف جديد
    /// Add new employee
    /// </summary>
    [RelayCommand]
    private void AddEmployee()
    {
        EditingEmployee = new Employee
        {
            EmployeeNumber = $"EMP{DateTime.Now:yyyyMMddHHmmss}",
            HireDate = DateTime.Now,
            EmploymentStatus = Core.Enums.EmploymentStatus.Active,
            IsActive = true
        };
        EmployeeImage = null;
        IsEditing = true;
    }

    /// <summary>
    /// تعديل موظف
    /// Edit employee
    /// </summary>
    [RelayCommand]
    private async Task EditEmployeeAsync()
    {
        if (SelectedEmployee == null) return;

        var employee = await _employeeService.GetWithDetailsAsync(SelectedEmployee.Id);
        EditingEmployee = employee ?? SelectedEmployee;
        LoadEmployeeImage(EditingEmployee?.ProfilePhoto);
        IsEditing = true;
    }

    /// <summary>
    /// إلغاء التحرير
    /// Cancel editing
    /// </summary>
    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        EditingEmployee = null;
        EmployeeImage = null;
    }

    /// <summary>
    /// حذف موظف
    /// Delete employee
    /// </summary>
    [RelayCommand]
    private async Task DeleteEmployeeAsync()
    {
        if (SelectedEmployee == null) return;

        await ExecuteAsync(async () =>
        {
            await _employeeService.DeleteAsync(SelectedEmployee.Id);
            Employees.Remove(SelectedEmployee);
            SelectedEmployee = null;
        }, "تم حذف الموظف بنجاح");
    }

    /// <summary>
    /// حفظ موظف
    /// Save employee
    /// </summary>
    [RelayCommand]
    private async Task SaveEmployeeAsync()
    {
        if (EditingEmployee == null) return;

        await ExecuteAsync(async () =>
        {
            if (EditingEmployee.Id == 0)
            {
                await _employeeService.CreateAsync(EditingEmployee);
            }
            else
            {
                await _employeeService.UpdateAsync(EditingEmployee);
            }
            IsEditing = false;
            EditingEmployee = null;
            EmployeeImage = null;
            await LoadEmployeesAsync();
        }, "تم حفظ بيانات الموظف بنجاح");
    }

    /// <summary>
    /// اختيار صورة الموظف
    /// Select employee image
    /// </summary>
    [RelayCommand]
    private void SelectImage()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "اختر صورة الموظف"
        };

        if (dialog.ShowDialog() == true && EditingEmployee != null)
        {
            try
            {
                // إنشاء مجلد الصور إذا لم يكن موجوداً
                if (!Directory.Exists(ImagesFolder))
                {
                    Directory.CreateDirectory(ImagesFolder);
                }

                // نسخ الصورة بإسم فريد
                var extension = Path.GetExtension(dialog.FileName);
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var destinationPath = Path.Combine(ImagesFolder, newFileName);

                File.Copy(dialog.FileName, destinationPath, true);

                // تحديث مسار الصورة في الموظف
                EditingEmployee.ProfilePhoto = newFileName;

                // تحميل الصورة للعرض
                LoadEmployeeImage(newFileName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"خطأ في تحميل الصورة: {ex.Message}", "خطأ",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// حذف صورة الموظف
    /// Remove employee image
    /// </summary>
    [RelayCommand]
    private void RemoveImage()
    {
        if (EditingEmployee != null)
        {
            EditingEmployee.ProfilePhoto = null;
            EmployeeImage = null;
        }
    }

    /// <summary>
    /// تحميل صورة الموظف
    /// Load employee image
    /// </summary>
    private void LoadEmployeeImage(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            EmployeeImage = null;
            return;
        }

        try
        {
            var fullPath = Path.Combine(ImagesFolder, imagePath);
            if (File.Exists(fullPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();
                EmployeeImage = bitmap;
            }
            else
            {
                EmployeeImage = null;
            }
        }
        catch
        {
            EmployeeImage = null;
        }
    }

    /// <summary>
    /// جلب مسار صورة الموظف الكامل
    /// Get full employee image path
    /// </summary>
    public static string? GetEmployeeImagePath(string? imageName)
    {
        if (string.IsNullOrEmpty(imageName)) return null;
        var path = Path.Combine(ImagesFolder, imageName);
        return File.Exists(path) ? path : null;
    }
}

