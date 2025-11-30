// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// ISpecificRepositories.cs - واجهات المستودعات المحددة
// =====================================================

using HRManagementSystem.Core.Models;

namespace HRManagementSystem.Core.Interfaces;

/// <summary>
/// واجهة مستودع الموظفين
/// Employee repository interface
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<Employee?> GetByNationalIdAsync(string nationalId);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm);
    Task<string> GenerateEmployeeNumberAsync();
    Task<Employee?> GetWithDetailsAsync(int id);
}

/// <summary>
/// واجهة مستودع الأقسام
/// Department repository interface
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetByNameAsync(string name);
    Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    Task<Department?> GetWithEmployeesAsync(int id);
    Task<IEnumerable<Department>> GetHierarchyAsync();
}

/// <summary>
/// واجهة مستودع المسميات الوظيفية
/// Position repository interface
/// </summary>
public interface IPositionRepository : IRepository<Position>
{
    Task<IEnumerable<Position>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<Position>> GetActivePositionsAsync();
}

/// <summary>
/// واجهة مستودع المستخدمين
/// User repository interface
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ValidateCredentialsAsync(string username, string password);
    Task UpdateLastLoginAsync(int userId);
    Task IncrementFailedAttemptsAsync(int userId);
    Task ResetFailedAttemptsAsync(int userId);
    Task LockAccountAsync(int userId, DateTime lockUntil);
}

/// <summary>
/// واجهة مستودع الحضور
/// Attendance repository interface
/// </summary>
public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<Attendance?> GetTodayAttendanceAsync(int employeeId);
    Task<IEnumerable<Attendance>> GetByDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date);
    Task<IEnumerable<Attendance>> GetDepartmentAttendanceAsync(int departmentId, DateTime date);
    Task<bool> CheckInAsync(int employeeId);
    Task<bool> CheckOutAsync(int employeeId);
    Task<Dictionary<string, int>> GetMonthlyStatsAsync(int employeeId, int year, int month);
}

/// <summary>
/// واجهة مستودع الإجازات
/// Leave repository interface
/// </summary>
public interface ILeaveRepository : IRepository<Leave>
{
    Task<IEnumerable<Leave>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<Leave>> GetPendingLeavesAsync();
    Task<IEnumerable<LeaveBalance>> GetBalancesAsync(int employeeId, int year);
    Task<bool> ApproveLeaveAsync(int leaveId, int approverId);
    Task<bool> RejectLeaveAsync(int leaveId, int approverId, string reason);
    Task<IEnumerable<LeaveType>> GetLeaveTypesAsync();
    Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveId = null);
}

/// <summary>
/// واجهة مستودع الرواتب
/// Payroll repository interface
/// </summary>
public interface IPayrollRepository : IRepository<Payroll>
{
    Task<Payroll?> GetByPeriodAsync(int employeeId, int year, int month);
    Task<IEnumerable<Payroll>> GetByPeriodAsync(int year, int month);
    Task<Payroll?> GetWithDetailsAsync(int id);
    Task<bool> GenerateMonthlyPayrollAsync(int year, int month, int createdBy);
    Task<bool> ApprovePayrollAsync(int payrollId, int approverId);
    Task<decimal> CalculateOvertimeAsync(int employeeId, int year, int month, decimal hourlyRate);
    Task<IEnumerable<SalaryComponent>> GetSalaryComponentsAsync();
    Task<IEnumerable<EmployeeSalaryComponent>> GetEmployeeSalaryComponentsAsync(int employeeId);
}

/// <summary>
/// واجهة مستودع المستندات
/// Document repository interface
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<Document>> GetExpiringDocumentsAsync(int daysUntilExpiry = 30);
    Task<IEnumerable<Document>> GetExpiredDocumentsAsync();
    Task<IEnumerable<Document>> GetByTypeAsync(int employeeId, string documentType);
}

