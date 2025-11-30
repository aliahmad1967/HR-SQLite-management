// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// BaseRepository.cs - المستودع الأساسي
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;
using System.Data;
using System.Linq.Expressions;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// المستودع الأساسي - يوفر العمليات الأساسية CRUD
/// Base repository - provides basic CRUD operations
/// </summary>
public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly IDatabaseContext _context;
    protected readonly string _tableName;

    protected BaseRepository(IDatabaseContext context, string tableName)
    {
        _context = context;
        _tableName = tableName;
    }

    /// <summary>
    /// جلب جميع السجلات
    /// Get all records
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var sql = $"SELECT * FROM {_tableName}";
        return await _context.Connection.QueryAsync<T>(sql, transaction: _context.Transaction);
    }

    /// <summary>
    /// جلب سجل بالمعرف
    /// Get record by ID
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return await _context.Connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id }, _context.Transaction);
    }

    /// <summary>
    /// البحث عن سجلات (تنفيذ أساسي)
    /// Find records (basic implementation)
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        // للتبسيط، نجلب كل السجلات ونطبق الفلتر في الذاكرة
        // للإنتاج، يفضل استخدام مكتبة لتحويل التعبير إلى SQL
        var all = await GetAllAsync();
        return all.AsQueryable().Where(predicate);
    }

    /// <summary>
    /// إضافة سجل جديد
    /// Add new record
    /// </summary>
    public virtual async Task<int> AddAsync(T entity)
    {
        var properties = GetInsertProperties();
        var columns = string.Join(", ", properties);
        var values = string.Join(", ", properties.Select(p => $"@{p}"));
        
        var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values}); SELECT last_insert_rowid();";
        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }

    /// <summary>
    /// إضافة مجموعة سجلات
    /// Add multiple records
    /// </summary>
    public virtual async Task<int> AddRangeAsync(IEnumerable<T> entities)
    {
        var count = 0;
        foreach (var entity in entities)
        {
            await AddAsync(entity);
            count++;
        }
        return count;
    }

    /// <summary>
    /// تحديث سجل
    /// Update record
    /// </summary>
    public virtual async Task<bool> UpdateAsync(T entity)
    {
        var properties = GetUpdateProperties();
        var setClause = string.Join(", ", properties.Select(p => $"{p} = @{p}"));
        
        var sql = $"UPDATE {_tableName} SET {setClause}, UpdatedAt = datetime('now') WHERE Id = @Id";
        var rows = await _context.Connection.ExecuteAsync(sql, entity, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// حذف سجل
    /// Delete record
    /// </summary>
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
        var rows = await _context.Connection.ExecuteAsync(sql, new { Id = id }, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// التحقق من وجود سجل
    /// Check if record exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE Id = @Id";
        var count = await _context.Connection.ExecuteScalarAsync<int>(sql, new { Id = id }, _context.Transaction);
        return count > 0;
    }

    /// <summary>
    /// عدد السجلات
    /// Count records
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            var sql = $"SELECT COUNT(*) FROM {_tableName}";
            return await _context.Connection.ExecuteScalarAsync<int>(sql, transaction: _context.Transaction);
        }
        
        var all = await FindAsync(predicate);
        return all.Count();
    }

    /// <summary>
    /// جلب أسماء الأعمدة للإدراج
    /// Get column names for insert
    /// </summary>
    protected virtual IEnumerable<string> GetInsertProperties()
    {
        return typeof(T).GetProperties()
            .Where(p => p.Name != "Id" && 
                       p.Name != "CreatedAt" && 
                       !p.PropertyType.IsClass || p.PropertyType == typeof(string))
            .Select(p => p.Name);
    }

    /// <summary>
    /// جلب أسماء الأعمدة للتحديث
    /// Get column names for update
    /// </summary>
    protected virtual IEnumerable<string> GetUpdateProperties()
    {
        return typeof(T).GetProperties()
            .Where(p => p.Name != "Id" && 
                       p.Name != "CreatedAt" && 
                       p.Name != "UpdatedAt" &&
                       (!p.PropertyType.IsClass || p.PropertyType == typeof(string)))
            .Select(p => p.Name);
    }
}

