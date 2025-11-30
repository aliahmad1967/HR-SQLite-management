// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// UserRepository.cs - مستودع المستخدمين
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع المستخدمين
/// User repository
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IDatabaseContext context) : base(context, "Users")
    {
    }

    /// <summary>
    /// جلب مستخدم باسم المستخدم
    /// Get user by username
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        return await _context.Connection.QueryFirstOrDefaultAsync<User>(sql, 
            new { Username = username }, _context.Transaction);
    }

    /// <summary>
    /// جلب مستخدم بالبريد الإلكتروني
    /// Get user by email
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM Users WHERE Email = @Email";
        return await _context.Connection.QueryFirstOrDefaultAsync<User>(sql, 
            new { Email = email }, _context.Transaction);
    }

    /// <summary>
    /// التحقق من بيانات الاعتماد
    /// Validate credentials
    /// </summary>
    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null || !user.IsActive || user.IsLocked)
            return false;

        // التحقق من كلمة المرور باستخدام BCrypt
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    /// <summary>
    /// تحديث آخر تسجيل دخول
    /// Update last login
    /// </summary>
    public async Task UpdateLastLoginAsync(int userId)
    {
        var sql = "UPDATE Users SET LastLoginAt = datetime('now'), FailedLoginAttempts = 0 WHERE Id = @UserId";
        await _context.Connection.ExecuteAsync(sql, new { UserId = userId }, _context.Transaction);
    }

    /// <summary>
    /// زيادة محاولات الدخول الفاشلة
    /// Increment failed login attempts
    /// </summary>
    public async Task IncrementFailedAttemptsAsync(int userId)
    {
        var sql = "UPDATE Users SET FailedLoginAttempts = FailedLoginAttempts + 1 WHERE Id = @UserId";
        await _context.Connection.ExecuteAsync(sql, new { UserId = userId }, _context.Transaction);
    }

    /// <summary>
    /// إعادة تعيين محاولات الدخول الفاشلة
    /// Reset failed login attempts
    /// </summary>
    public async Task ResetFailedAttemptsAsync(int userId)
    {
        var sql = "UPDATE Users SET FailedLoginAttempts = 0, LockedUntil = NULL WHERE Id = @UserId";
        await _context.Connection.ExecuteAsync(sql, new { UserId = userId }, _context.Transaction);
    }

    /// <summary>
    /// قفل الحساب
    /// Lock account
    /// </summary>
    public async Task LockAccountAsync(int userId, DateTime lockUntil)
    {
        var sql = "UPDATE Users SET LockedUntil = @LockUntil WHERE Id = @UserId";
        await _context.Connection.ExecuteAsync(sql, new { UserId = userId, LockUntil = lockUntil }, _context.Transaction);
    }

    /// <summary>
    /// إضافة مستخدم جديد
    /// Add new user
    /// </summary>
    public override async Task<int> AddAsync(User entity)
    {
        var sql = @"INSERT INTO Users (Username, PasswordHash, Email, EmployeeId, Role, IsActive)
                    VALUES (@Username, @PasswordHash, @Email, @EmployeeId, @Role, @IsActive);
                    SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }

    /// <summary>
    /// تغيير كلمة المرور
    /// Change password
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash)
    {
        var sql = "UPDATE Users SET PasswordHash = @PasswordHash, UpdatedAt = datetime('now') WHERE Id = @UserId";
        var rows = await _context.Connection.ExecuteAsync(sql, 
            new { UserId = userId, PasswordHash = newPasswordHash }, _context.Transaction);
        return rows > 0;
    }
}

