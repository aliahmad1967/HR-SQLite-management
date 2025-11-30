// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// AuthenticationService.cs - خدمة المصادقة
// =====================================================

using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Serilog;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة المصادقة
/// Authentication service
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 30;

    public User? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;

    public AuthenticationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = Log.ForContext<AuthenticationService>();
    }

    /// <summary>
    /// تسجيل الدخول
    /// Login
    /// </summary>
    public async Task<(bool Success, User? User, string Message)> LoginAsync(string username, string password)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.Warning("محاولة تسجيل دخول فاشلة - المستخدم غير موجود: {Username}", username);
                return (false, null, "اسم المستخدم أو كلمة المرور غير صحيحة");
            }

            // التحقق من حالة الحساب
            if (!user.IsActive)
            {
                _logger.Warning("محاولة تسجيل دخول لحساب غير نشط: {Username}", username);
                return (false, null, "الحساب غير نشط. يرجى التواصل مع المسؤول");
            }

            // التحقق من القفل
            if (user.IsLocked)
            {
                var remainingMinutes = (int)(user.LockedUntil!.Value - DateTime.Now).TotalMinutes;
                _logger.Warning("محاولة تسجيل دخول لحساب مقفل: {Username}", username);
                return (false, null, $"الحساب مقفل. يرجى المحاولة بعد {remainingMinutes} دقيقة");
            }

            // التحقق من كلمة المرور
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                await _unitOfWork.Users.IncrementFailedAttemptsAsync(user.Id);

                if (user.FailedLoginAttempts + 1 >= MaxFailedAttempts)
                {
                    await _unitOfWork.Users.LockAccountAsync(user.Id, DateTime.Now.AddMinutes(LockoutMinutes));
                    _logger.Warning("تم قفل الحساب بسبب محاولات فاشلة متعددة: {Username}", username);
                    return (false, null, $"تم قفل الحساب لمدة {LockoutMinutes} دقيقة بسبب محاولات فاشلة متعددة");
                }

                _logger.Warning("كلمة مرور خاطئة للمستخدم: {Username}", username);
                return (false, null, "اسم المستخدم أو كلمة المرور غير صحيحة");
            }

            // تسجيل الدخول الناجح
            await _unitOfWork.Users.UpdateLastLoginAsync(user.Id);
            CurrentUser = user;

            _logger.Information("تسجيل دخول ناجح: {Username}", username);
            return (true, user, "تم تسجيل الدخول بنجاح");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تسجيل الدخول: {Username}", username);
            return (false, null, "حدث خطأ أثناء تسجيل الدخول");
        }
    }

    /// <summary>
    /// تسجيل الخروج
    /// Logout
    /// </summary>
    public async Task<bool> LogoutAsync()
    {
        if (CurrentUser != null)
        {
            _logger.Information("تسجيل خروج: {Username}", CurrentUser.Username);
            CurrentUser = null;
        }
        return await Task.FromResult(true);
    }

    /// <summary>
    /// تغيير كلمة المرور
    /// Change password
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                _logger.Warning("محاولة تغيير كلمة مرور بكلمة مرور حالية خاطئة: {UserId}", userId);
                return false;
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = newHash;
            await _unitOfWork.Users.UpdateAsync(user);

            _logger.Information("تم تغيير كلمة المرور بنجاح: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء تغيير كلمة المرور: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// إعادة تعيين كلمة المرور
    /// Reset password
    /// </summary>
    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = newHash;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.Users.ResetFailedAttemptsAsync(userId);

            _logger.Information("تم إعادة تعيين كلمة المرور: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء إعادة تعيين كلمة المرور: {UserId}", userId);
            return false;
        }
    }
}

