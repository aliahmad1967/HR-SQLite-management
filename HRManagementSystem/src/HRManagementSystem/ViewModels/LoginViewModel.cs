// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// LoginViewModel.cs - ViewModel تسجيل الدخول
// =====================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRManagementSystem.Services.Interfaces;

namespace HRManagementSystem.ViewModels;

/// <summary>
/// ViewModel تسجيل الدخول
/// Login ViewModel
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// اسم المستخدم
    /// Username
    /// </summary>
    [ObservableProperty]
    private string _username = string.Empty;

    /// <summary>
    /// كلمة المرور
    /// Password
    /// </summary>
    [ObservableProperty]
    private string _password = string.Empty;

    /// <summary>
    /// تذكرني
    /// Remember me
    /// </summary>
    [ObservableProperty]
    private bool _rememberMe;

    /// <summary>
    /// حدث تسجيل الدخول الناجح
    /// Login success event
    /// </summary>
    public event EventHandler? LoginSuccessful;

    public LoginViewModel(IAuthenticationService authService)
    {
        _authService = authService;
        Title = "تسجيل الدخول";
    }

    /// <summary>
    /// هل يمكن تسجيل الدخول؟
    /// Can login?
    /// </summary>
    private bool CanLogin => !string.IsNullOrWhiteSpace(Username) && 
                             !string.IsNullOrWhiteSpace(Password) && 
                             !IsBusy;

    /// <summary>
    /// تسجيل الدخول
    /// Login
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        await ExecuteAsync(async () =>
        {
            var (success, user, message) = await _authService.LoginAsync(Username, Password);

            if (success)
            {
                ShowSuccess(message);
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ShowError(message);
            }
        });
    }

    /// <summary>
    /// عند تغيير اسم المستخدم أو كلمة المرور
    /// On username or password changed
    /// </summary>
    partial void OnUsernameChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
}

