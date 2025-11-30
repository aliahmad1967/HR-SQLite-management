// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// LoginWindow.xaml.cs - نافذة تسجيل الدخول
// =====================================================

using HRManagementSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace HRManagementSystem.Views;

/// <summary>
/// نافذة تسجيل الدخول
/// Login Window
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // الاشتراك في حدث تسجيل الدخول الناجح
        _viewModel.LoginSuccessful += OnLoginSuccessful;
    }

    /// <summary>
    /// عند تغيير كلمة المرور
    /// On password changed
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.Password = passwordBox.Password;
        }
    }

    /// <summary>
    /// عند نجاح تسجيل الدخول
    /// On login successful
    /// </summary>
    private void OnLoginSuccessful(object? sender, EventArgs e)
    {
        // فتح النافذة الرئيسية
        var mainWindow = App.GetService<MainWindow>();
        mainWindow.Show();

        // إغلاق نافذة تسجيل الدخول
        Close();
    }

    /// <summary>
    /// عند إغلاق النافذة
    /// On window closing
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        _viewModel.LoginSuccessful -= OnLoginSuccessful;
        base.OnClosed(e);
    }
}

