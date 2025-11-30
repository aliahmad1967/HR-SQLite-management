// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// IServices.cs - واجهات الخدمات
// =====================================================

using HRManagementSystem.Core.Models;

namespace HRManagementSystem.Services.Interfaces;

/// <summary>
/// واجهة خدمة المصادقة
/// Authentication service interface
/// </summary>
public interface IAuthenticationService
{
    Task<(bool Success, User? User, string Message)> LoginAsync(string username, string password);
    Task<bool> LogoutAsync();
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(int userId, string newPassword);
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
}

/// <summary>
/// واجهة خدمة الموظفين
/// Employee service interface
/// </summary>
public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<int> CreateAsync(Employee employee);
    Task<bool> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(int id);
    Task<bool> TerminateAsync(int id, string reason);
}

/// <summary>
/// واجهة خدمة الحضور
/// Attendance service interface
/// </summary>
public interface IAttendanceService
{
    Task<bool> CheckInAsync(int employeeId);
    Task<bool> CheckOutAsync(int employeeId);
    Task<Attendance?> GetTodayAttendanceAsync(int employeeId);
    Task<IEnumerable<Attendance>> GetByDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Attendance>> GetDepartmentAttendanceAsync(int departmentId, DateTime date);
    Task<Dictionary<string, int>> GetMonthlyStatsAsync(int employeeId, int year, int month);
    Task<bool> AddManualAttendanceAsync(Attendance attendance);
}

/// <summary>
/// واجهة خدمة الإجازات
/// Leave service interface
/// </summary>
public interface ILeaveService
{
    Task<IEnumerable<Leave>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<Leave>> GetPendingLeavesAsync();
    Task<IEnumerable<LeaveBalance>> GetBalancesAsync(int employeeId, int year);
    Task<IEnumerable<LeaveType>> GetLeaveTypesAsync();
    Task<int> RequestLeaveAsync(Leave leave);
    Task<bool> ApproveLeaveAsync(int leaveId, int approverId);
    Task<bool> RejectLeaveAsync(int leaveId, int approverId, string reason);
    Task<bool> CancelLeaveAsync(int leaveId);
    Task<(bool IsValid, string Message)> ValidateLeaveRequestAsync(Leave leave);
}

/// <summary>
/// واجهة خدمة الرواتب
/// Payroll service interface
/// </summary>
public interface IPayrollService
{
    Task<IEnumerable<Payroll>> GetByPeriodAsync(int year, int month);
    Task<Payroll?> GetWithDetailsAsync(int id);
    Task<bool> GenerateMonthlyPayrollAsync(int year, int month, int createdBy);
    Task<bool> CalculatePayrollAsync(int payrollId);
    Task<bool> ApprovePayrollAsync(int payrollId, int approverId);
    Task<bool> ProcessPaymentAsync(int payrollId);
    Task<byte[]> GeneratePayslipAsync(int payrollId);
    Task<IEnumerable<SalaryComponent>> GetSalaryComponentsAsync();
}

/// <summary>
/// واجهة خدمة الأقسام
/// Department service interface
/// </summary>
public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department?> GetWithEmployeesAsync(int id);
    Task<int> CreateAsync(Department department);
    Task<bool> UpdateAsync(Department department);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// واجهة خدمة المستندات
/// Document service interface
/// </summary>
public interface IDocumentService
{
    Task<IEnumerable<Document>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<Document>> GetExpiringDocumentsAsync(int daysUntilExpiry = 30);
    Task<IEnumerable<Document>> GetExpiredDocumentsAsync();
    Task<int> UploadDocumentAsync(Document document, Stream fileStream);
    Task<Stream?> DownloadDocumentAsync(int documentId);
    Task<bool> DeleteDocumentAsync(int id);
}

/// <summary>
/// واجهة خدمة التقارير
/// Report service interface
/// </summary>
public interface IReportService
{
    Task<byte[]> GenerateEmployeeReportAsync();
    Task<byte[]> GenerateAttendanceReportAsync(DateTime startDate, DateTime endDate);
    Task<byte[]> GeneratePayrollReportAsync(int year, int month);
    Task<byte[]> GenerateLeaveReportAsync(int year);
    Task<DashboardStats> GetDashboardStatsAsync();
}

/// <summary>
/// إحصائيات لوحة التحكم
/// Dashboard statistics
/// </summary>
public class DashboardStats
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public int OnLeaveToday { get; set; }
    public int PendingLeaveRequests { get; set; }
    public decimal TotalPayrollThisMonth { get; set; }
    public int ExpiringDocuments { get; set; }
    public List<DepartmentStats> DepartmentStats { get; set; } = new();
    public List<MonthlyAttendanceStats> MonthlyAttendance { get; set; } = new();
}

public class DepartmentStats
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class MonthlyAttendanceStats
{
    public string Month { get; set; } = string.Empty;
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
}

/// <summary>
/// واجهة خدمة النسخ الاحتياطي
/// Backup service interface
/// </summary>
public interface IBackupService
{
    Task<string> CreateBackupAsync();
    Task RestoreBackupAsync(string backupPath);
    Task<string[]> GetBackupsAsync();
    Task CleanupOldBackupsAsync(int keepCount = 10);
}

