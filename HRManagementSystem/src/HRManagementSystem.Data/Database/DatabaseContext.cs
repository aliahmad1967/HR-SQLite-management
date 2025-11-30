// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DatabaseContext.cs - سياق قاعدة البيانات
// =====================================================

using HRManagementSystem.Core.Interfaces;
using Microsoft.Data.Sqlite;
using System.Data;

namespace HRManagementSystem.Data.Database;

/// <summary>
/// سياق قاعدة البيانات - يدير الاتصال بـ SQLite
/// Database context - manages SQLite connection
/// </summary>
public class DatabaseContext : IDatabaseContext
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;
    private SqliteTransaction? _transaction;
    private bool _disposed;

    /// <summary>
    /// منشئ سياق قاعدة البيانات
    /// Database context constructor
    /// </summary>
    public DatabaseContext(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
    }

    /// <summary>
    /// الحصول على الاتصال
    /// Get connection
    /// </summary>
    public IDbConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                // تفعيل المفاتيح الخارجية
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();
            }

            return _connection;
        }
    }

    /// <summary>
    /// الحصول على المعاملة الحالية
    /// Get current transaction
    /// </summary>
    public IDbTransaction? Transaction => _transaction;

    /// <summary>
    /// بدء معاملة جديدة
    /// Begin new transaction
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            var conn = Connection;
        }
        _transaction = _connection!.BeginTransaction();
        await Task.CompletedTask;
    }

    /// <summary>
    /// تأكيد المعاملة
    /// Commit transaction
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
        await Task.CompletedTask;
    }

    /// <summary>
    /// التراجع عن المعاملة
    /// Rollback transaction
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
        await Task.CompletedTask;
    }

    /// <summary>
    /// تنفيذ استعلام SQL مباشر
    /// Execute raw SQL query
    /// </summary>
    public async Task<int> ExecuteAsync(string sql)
    {
        using var cmd = _connection!.CreateCommand();
        cmd.CommandText = sql;
        return await Task.FromResult(cmd.ExecuteNonQuery());
    }

    /// <summary>
    /// التخلص من الموارد
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }
}

