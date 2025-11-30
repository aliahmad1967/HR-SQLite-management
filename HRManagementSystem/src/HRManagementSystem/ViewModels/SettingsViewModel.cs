// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// SettingsViewModel.cs - ViewModel الإعدادات
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Services.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel للإعدادات
/// Settings ViewModel
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private readonly IBackupService _backupService;
    private readonly IAuthenticationService _authService;
    private readonly PaletteHelper _paletteHelper = new();

    [ObservableProperty] private string _currentPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isSuccess;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private ObservableCollection<BackupInfo> _backups = new();
    [ObservableProperty] private bool _isDarkMode;

    public SettingsViewModel(IBackupService backupService, IAuthenticationService authService)
    {
        _backupService = backupService;
        _authService = authService;

        // تحميل الوضع الحالي من الإعدادات
        IsDarkMode = Properties.Settings.Default.IsDarkMode;

        _ = LoadBackupsAsync();
    }

    /// <summary>
    /// تبديل الوضع الداكن/الفاتح
    /// Toggle dark/light mode
    /// </summary>
    partial void OnIsDarkModeChanged(bool value)
    {
        var theme = _paletteHelper.GetTheme();
        theme.SetBaseTheme(value ? Theme.Dark : Theme.Light);
        _paletteHelper.SetTheme(theme);

        // حفظ الإعداد
        Properties.Settings.Default.IsDarkMode = value;
        Properties.Settings.Default.Save();
    }

    private async Task LoadBackupsAsync()
    {
        try
        {
            var files = await _backupService.GetBackupsAsync();
            Backups.Clear();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var info = new FileInfo(file);
                Backups.Add(new BackupInfo
                {
                    FilePath = file,
                    FileName = info.Name,
                    CreatedDate = info.CreationTime,
                    Size = $"{info.Length / 1024.0:F2} KB"
                });
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"خطأ في تحميل النسخ الاحتياطية: {ex.Message}";
            IsSuccess = false;
        }
    }

    [RelayCommand]
    private async Task CreateBackupAsync()
    {
        try
        {
            IsBusy = true;
            var path = await _backupService.CreateBackupAsync();
            StatusMessage = $"تم إنشاء النسخة الاحتياطية بنجاح";
            IsSuccess = true;
            await LoadBackupsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"خطأ: {ex.Message}";
            IsSuccess = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RestoreBackupAsync(BackupInfo? backup)
    {
        if (backup == null) return;

        var result = MessageBox.Show(
            "هل أنت متأكد من استعادة هذه النسخة الاحتياطية؟\nسيتم إنشاء نسخة احتياطية من البيانات الحالية قبل الاستعادة.",
            "تأكيد الاستعادة",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            IsBusy = true;
            await _backupService.RestoreBackupAsync(backup.FilePath);
            StatusMessage = "تم استعادة النسخة الاحتياطية بنجاح. يرجى إعادة تشغيل التطبيق.";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"خطأ: {ex.Message}";
            IsSuccess = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword))
        {
            StatusMessage = "يرجى ملء جميع الحقول";
            IsSuccess = false;
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            StatusMessage = "كلمة المرور الجديدة غير متطابقة";
            IsSuccess = false;
            return;
        }

        try
        {
            IsBusy = true;
            var userId = _authService.CurrentUser?.Id ?? 0;
            var success = await _authService.ChangePasswordAsync(userId, CurrentPassword, NewPassword);
            
            if (success)
            {
                StatusMessage = "تم تغيير كلمة المرور بنجاح";
                IsSuccess = true;
                CurrentPassword = NewPassword = ConfirmPassword = string.Empty;
            }
            else
            {
                StatusMessage = "كلمة المرور الحالية غير صحيحة";
                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"خطأ: {ex.Message}";
            IsSuccess = false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public class BackupInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Size { get; set; } = string.Empty;
}

