// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// BackupService.cs - خدمة النسخ الاحتياطي
// =====================================================

using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Services.Interfaces;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة النسخ الاحتياطي
/// Backup Service
/// </summary>
public class BackupService : IBackupService
{
    private readonly IDatabaseContext _context;
    private readonly string _backupFolder;

    public BackupService(IDatabaseContext context)
    {
        _context = context;
        _backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        
        if (!Directory.Exists(_backupFolder))
        {
            Directory.CreateDirectory(_backupFolder);
        }
    }

    /// <summary>
    /// إنشاء نسخة احتياطية
    /// Create backup
    /// </summary>
    public async Task<string> CreateBackupAsync()
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFileName = $"hr_backup_{timestamp}.db";
            var backupPath = Path.Combine(_backupFolder, backupFileName);

            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "hr_database.db");

            if (File.Exists(dbPath))
            {
                await Task.Run(() => File.Copy(dbPath, backupPath, true));
                Log.Information("تم إنشاء نسخة احتياطية: {BackupPath}", backupPath);
                return backupPath;
            }

            throw new FileNotFoundException("قاعدة البيانات غير موجودة");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "خطأ في إنشاء النسخة الاحتياطية");
            throw;
        }
    }

    /// <summary>
    /// استعادة من نسخة احتياطية
    /// Restore from backup
    /// </summary>
    public async Task RestoreBackupAsync(string backupPath)
    {
        try
        {
            if (!File.Exists(backupPath))
            {
                throw new FileNotFoundException("ملف النسخة الاحتياطية غير موجود");
            }

            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "hr_database.db");

            // Create backup of current before restore
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var preRestoreBackup = Path.Combine(_backupFolder, $"pre_restore_{timestamp}.db");
            
            if (File.Exists(dbPath))
            {
                await Task.Run(() => File.Copy(dbPath, preRestoreBackup, true));
            }

            await Task.Run(() => File.Copy(backupPath, dbPath, true));
            Log.Information("تم استعادة النسخة الاحتياطية من: {BackupPath}", backupPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "خطأ في استعادة النسخة الاحتياطية");
            throw;
        }
    }

    /// <summary>
    /// الحصول على قائمة النسخ الاحتياطية
    /// Get list of backups
    /// </summary>
    public async Task<string[]> GetBackupsAsync()
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(_backupFolder))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(_backupFolder, "*.db");
        });
    }

    /// <summary>
    /// حذف النسخ الاحتياطية القديمة
    /// Delete old backups (keep last N)
    /// </summary>
    public async Task CleanupOldBackupsAsync(int keepCount = 10)
    {
        await Task.Run(() =>
        {
            var files = new DirectoryInfo(_backupFolder)
                .GetFiles("hr_backup_*.db")
                .OrderByDescending(f => f.CreationTime)
                .Skip(keepCount)
                .ToList();

            foreach (var file in files)
            {
                file.Delete();
                Log.Information("تم حذف نسخة احتياطية قديمة: {FileName}", file.Name);
            }
        });
    }
}

