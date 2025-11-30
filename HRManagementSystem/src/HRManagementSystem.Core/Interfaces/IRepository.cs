// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// IRepository.cs - واجهة المستودع العامة
// =====================================================

using System.Linq.Expressions;

namespace HRManagementSystem.Core.Interfaces;

/// <summary>
/// واجهة المستودع العامة
/// Generic repository interface
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// جلب جميع السجلات
    /// Get all records
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// جلب سجل بالمعرف
    /// Get record by ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// البحث عن سجلات
    /// Find records
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// إضافة سجل
    /// Add record
    /// </summary>
    Task<int> AddAsync(T entity);

    /// <summary>
    /// إضافة مجموعة سجلات
    /// Add multiple records
    /// </summary>
    Task<int> AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// تحديث سجل
    /// Update record
    /// </summary>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// حذف سجل
    /// Delete record
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// التحقق من وجود سجل
    /// Check if record exists
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// عدد السجلات
    /// Count records
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

/// <summary>
/// واجهة وحدة العمل
/// Unit of work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    IPositionRepository Positions { get; }
    IUserRepository Users { get; }
    IAttendanceRepository Attendance { get; }
    ILeaveRepository Leaves { get; }
    IPayrollRepository Payroll { get; }
    IDocumentRepository Documents { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

/// <summary>
/// واجهة سياق قاعدة البيانات
/// Database context interface
/// </summary>
public interface IDatabaseContext : IDisposable
{
    /// <summary>
    /// الاتصال بقاعدة البيانات
    /// Database connection
    /// </summary>
    System.Data.IDbConnection Connection { get; }

    /// <summary>
    /// المعاملة الحالية
    /// Current transaction
    /// </summary>
    System.Data.IDbTransaction? Transaction { get; }

    /// <summary>
    /// بدء معاملة
    /// Begin transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// تأكيد المعاملة
    /// Commit transaction
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// التراجع عن المعاملة
    /// Rollback transaction
    /// </summary>
    Task RollbackTransactionAsync();
}

