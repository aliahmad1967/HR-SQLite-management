// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// UnitOfWork.cs - وحدة العمل
// =====================================================

using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// وحدة العمل - تدير المعاملات والمستودعات
/// Unit of work - manages transactions and repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDatabaseContext _context;
    private bool _disposed;

    private IEmployeeRepository? _employees;
    private IDepartmentRepository? _departments;
    private IPositionRepository? _positions;
    private IUserRepository? _users;
    private IAttendanceRepository? _attendance;
    private ILeaveRepository? _leaves;
    private IPayrollRepository? _payroll;
    private IDocumentRepository? _documents;

    public UnitOfWork(IDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// مستودع الموظفين
    /// Employee repository
    /// </summary>
    public IEmployeeRepository Employees => 
        _employees ??= new EmployeeRepository(_context);

    /// <summary>
    /// مستودع الأقسام
    /// Department repository
    /// </summary>
    public IDepartmentRepository Departments => 
        _departments ??= new DepartmentRepository(_context);

    /// <summary>
    /// مستودع المسميات الوظيفية
    /// Position repository
    /// </summary>
    public IPositionRepository Positions => 
        _positions ??= new PositionRepository(_context);

    /// <summary>
    /// مستودع المستخدمين
    /// User repository
    /// </summary>
    public IUserRepository Users => 
        _users ??= new UserRepository(_context);

    /// <summary>
    /// مستودع الحضور
    /// Attendance repository
    /// </summary>
    public IAttendanceRepository Attendance => 
        _attendance ??= new AttendanceRepository(_context);

    /// <summary>
    /// مستودع الإجازات
    /// Leave repository
    /// </summary>
    public ILeaveRepository Leaves => 
        _leaves ??= new LeaveRepository(_context);

    /// <summary>
    /// مستودع الرواتب
    /// Payroll repository
    /// </summary>
    public IPayrollRepository Payroll => 
        _payroll ??= new PayrollRepository(_context);

    /// <summary>
    /// مستودع المستندات
    /// Document repository
    /// </summary>
    public IDocumentRepository Documents => 
        _documents ??= new DocumentRepository(_context);

    /// <summary>
    /// حفظ التغييرات
    /// Save changes
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        // في Dapper، التغييرات تُحفظ مباشرة
        // مع المعاملات، نحتاج فقط للتأكيد
        await Task.CompletedTask;
        return 0;
    }

    /// <summary>
    /// بدء معاملة
    /// Begin transaction
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        await _context.BeginTransactionAsync();
    }

    /// <summary>
    /// تأكيد المعاملة
    /// Commit transaction
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        await _context.CommitTransactionAsync();
    }

    /// <summary>
    /// التراجع عن المعاملة
    /// Rollback transaction
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        await _context.RollbackTransactionAsync();
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
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}

