// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// BaseViewModel.cs - ViewModel الأساسي
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel الأساسي - يوفر الوظائف المشتركة
/// Base ViewModel - provides common functionality
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    /// <summary>
    /// هل يتم التحميل؟
    /// Is loading?
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    /// <summary>
    /// هل غير مشغول؟
    /// Is not busy?
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// عنوان الصفحة
    /// Page title
    /// </summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>
    /// رسالة الخطأ
    /// Error message
    /// </summary>
    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// رسالة النجاح
    /// Success message
    /// </summary>
    [ObservableProperty]
    private string? _successMessage;

    /// <summary>
    /// هل يوجد خطأ؟
    /// Has error?
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// هل يوجد رسالة نجاح؟
    /// Has success message?
    /// </summary>
    public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

    /// <summary>
    /// مسح الرسائل
    /// Clear messages
    /// </summary>
    protected void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }

    /// <summary>
    /// عرض رسالة خطأ
    /// Show error message
    /// </summary>
    protected void ShowError(string message)
    {
        ClearMessages();
        ErrorMessage = message;
    }

    /// <summary>
    /// عرض رسالة نجاح
    /// Show success message
    /// </summary>
    protected void ShowSuccess(string message)
    {
        ClearMessages();
        SuccessMessage = message;
    }

    /// <summary>
    /// تنفيذ عملية مع معالجة الأخطاء
    /// Execute operation with error handling
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation, string? successMessage = null)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearMessages();
            await operation();

            if (!string.IsNullOrEmpty(successMessage))
                ShowSuccess(successMessage);
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// تنفيذ عملية مع إرجاع نتيجة
    /// Execute operation with result
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (IsBusy)
            return default;

        try
        {
            IsBusy = true;
            ClearMessages();
            return await operation();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// تحميل البيانات
    /// Load data
    /// </summary>
    public virtual Task LoadAsync() => Task.CompletedTask;

    /// <summary>
    /// تحديث البيانات
    /// Refresh data
    /// </summary>
    [RelayCommand]
    public virtual async Task RefreshAsync()
    {
        await LoadAsync();
    }
}

