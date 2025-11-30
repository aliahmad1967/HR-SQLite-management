// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DatabaseInitializer.cs - مهيئ قاعدة البيانات
// =====================================================

using HRManagementSystem.Core.Interfaces;
using Microsoft.Data.Sqlite;

namespace HRManagementSystem.Data.Database;

/// <summary>
/// مهيئ قاعدة البيانات - ينشئ الجداول والبيانات الأولية
/// Database initializer - creates tables and seed data
/// </summary>
public class DatabaseInitializer
{
    private readonly IDatabaseContext _context;

    public DatabaseInitializer(IDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// تهيئة قاعدة البيانات
    /// Initialize database
    /// </summary>
    public async Task InitializeAsync()
    {
        var connection = _context.Connection as SqliteConnection;
        if (connection == null) return;

        // التحقق من وجود الجداول
        using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Departments';";
        var result = await checkCmd.ExecuteScalarAsync();

        if (result != null) return; // الجداول موجودة

        // تنفيذ سكربتات إنشاء الجداول
        await ExecuteScriptAsync(connection, CreateTablesScript);
        await ExecuteScriptAsync(connection, CreateAttendanceTablesScript);
        await ExecuteScriptAsync(connection, CreatePayrollTablesScript);
        await ExecuteScriptAsync(connection, SeedDataScript);
    }

    private async Task ExecuteScriptAsync(SqliteConnection connection, string script)
    {
        if (string.IsNullOrWhiteSpace(script)) return;

        using var cmd = connection.CreateCommand();
        cmd.CommandText = script;
        await cmd.ExecuteNonQueryAsync();
    }

    #region SQL Scripts
    private const string CreateTablesScript = @"
        CREATE TABLE IF NOT EXISTS Departments (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Description TEXT,
            ManagerId INTEGER,
            IsActive INTEGER DEFAULT 1,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TABLE IF NOT EXISTS Positions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Description TEXT,
            DepartmentId INTEGER,
            MinSalary REAL DEFAULT 0,
            MaxSalary REAL DEFAULT 0,
            IsActive INTEGER DEFAULT 1,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
        );

        CREATE TABLE IF NOT EXISTS Employees (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeNumber TEXT UNIQUE NOT NULL,
            FirstName TEXT NOT NULL,
            LastName TEXT NOT NULL,
            Email TEXT UNIQUE,
            Phone TEXT,
            DateOfBirth TEXT,
            Gender TEXT,
            NationalId TEXT UNIQUE,
            Address TEXT,
            City TEXT,
            DepartmentId INTEGER,
            PositionId INTEGER,
            HireDate TEXT NOT NULL,
            TerminationDate TEXT,
            EmploymentStatus TEXT DEFAULT 'Active',
            BasicSalary REAL DEFAULT 0,
            ManagerId INTEGER,
            IsActive INTEGER DEFAULT 1,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (DepartmentId) REFERENCES Departments(Id),
            FOREIGN KEY (PositionId) REFERENCES Positions(Id),
            FOREIGN KEY (ManagerId) REFERENCES Employees(Id)
        );

        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT UNIQUE NOT NULL,
            PasswordHash TEXT NOT NULL,
            EmployeeId INTEGER,
            Role TEXT DEFAULT 'Employee',
            IsActive INTEGER DEFAULT 1,
            LastLoginAt TEXT,
            FailedLoginAttempts INTEGER DEFAULT 0,
            LockoutEnd TEXT,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
        );
    ";

    private const string CreateAttendanceTablesScript = @"
        CREATE TABLE IF NOT EXISTS Attendance (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeId INTEGER NOT NULL,
            Date TEXT NOT NULL,
            CheckInTime TEXT,
            CheckOutTime TEXT,
            WorkHours REAL DEFAULT 0,
            OvertimeHours REAL DEFAULT 0,
            Status TEXT DEFAULT 'Present',
            Notes TEXT,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
        );

        CREATE TABLE IF NOT EXISTS LeaveTypes (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Description TEXT,
            DefaultDays INTEGER DEFAULT 0,
            IsPaid INTEGER DEFAULT 1,
            IsActive INTEGER DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS Leaves (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeId INTEGER NOT NULL,
            LeaveTypeId INTEGER NOT NULL,
            StartDate TEXT NOT NULL,
            EndDate TEXT NOT NULL,
            TotalDays INTEGER NOT NULL,
            Reason TEXT,
            Status TEXT DEFAULT 'Pending',
            ApprovedById INTEGER,
            ApprovedAt TEXT,
            RejectionReason TEXT,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
            FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id),
            FOREIGN KEY (ApprovedById) REFERENCES Users(Id)
        );

        CREATE TABLE IF NOT EXISTS LeaveBalances (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeId INTEGER NOT NULL,
            LeaveTypeId INTEGER NOT NULL,
            Year INTEGER NOT NULL,
            TotalDays INTEGER DEFAULT 0,
            UsedDays INTEGER DEFAULT 0,
            RemainingDays INTEGER DEFAULT 0,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
            FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id)
        );
    ";

    private const string CreatePayrollTablesScript = @"
        CREATE TABLE IF NOT EXISTS Payroll (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeId INTEGER NOT NULL,
            Month INTEGER NOT NULL,
            Year INTEGER NOT NULL,
            BasicSalary REAL DEFAULT 0,
            TotalAllowances REAL DEFAULT 0,
            TotalDeductions REAL DEFAULT 0,
            OvertimeAmount REAL DEFAULT 0,
            GrossSalary REAL DEFAULT 0,
            NetSalary REAL DEFAULT 0,
            Status TEXT DEFAULT 'Draft',
            ProcessedAt TEXT,
            ProcessedById INTEGER,
            PaidAt TEXT,
            Notes TEXT,
            CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
            FOREIGN KEY (ProcessedById) REFERENCES Users(Id)
        );

        CREATE TABLE IF NOT EXISTS PayrollDetails (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PayrollId INTEGER NOT NULL,
            ComponentType TEXT NOT NULL,
            ComponentName TEXT NOT NULL,
            Amount REAL DEFAULT 0,
            IsDeduction INTEGER DEFAULT 0,
            FOREIGN KEY (PayrollId) REFERENCES Payroll(Id)
        );

        CREATE TABLE IF NOT EXISTS Documents (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeId INTEGER NOT NULL,
            DocumentType TEXT NOT NULL,
            FileName TEXT NOT NULL,
            FilePath TEXT NOT NULL,
            FileSize INTEGER DEFAULT 0,
            UploadedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            UploadedById INTEGER,
            Notes TEXT,
            FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
            FOREIGN KEY (UploadedById) REFERENCES Users(Id)
        );
    ";

    private const string SeedDataScript = @"
        -- إدخال أنواع الإجازات (5 أنواع)
        INSERT OR IGNORE INTO LeaveTypes (Id, Name, Description, DefaultDays, IsPaid) VALUES
        (1, 'إجازة سنوية', 'الإجازة السنوية المدفوعة', 21, 1),
        (2, 'إجازة مرضية', 'إجازة مرضية بتقرير طبي', 30, 1),
        (3, 'إجازة بدون راتب', 'إجازة غير مدفوعة', 0, 0),
        (4, 'إجازة زواج', 'إجازة الزواج', 5, 1),
        (5, 'إجازة وفاة', 'إجازة وفاة قريب', 3, 1);

        -- إدخال 5 أقسام
        INSERT OR IGNORE INTO Departments (Id, Name, Description) VALUES
        (1, 'الإدارة العامة', 'القسم الإداري الرئيسي'),
        (2, 'الموارد البشرية', 'قسم شؤون الموظفين والتوظيف'),
        (3, 'المالية والمحاسبة', 'قسم الحسابات والمالية'),
        (4, 'تقنية المعلومات', 'قسم الدعم الفني والتطوير'),
        (5, 'التسويق والمبيعات', 'قسم التسويق وخدمة العملاء');

        -- إدخال 5 مناصب
        INSERT OR IGNORE INTO Positions (Id, Title, Description, DepartmentId, MinSalary, MaxSalary) VALUES
        (1, 'مدير عام', 'المدير العام للشركة', 1, 2000000, 5000000),
        (2, 'مدير موارد بشرية', 'مسؤول قسم الموارد البشرية', 2, 1500000, 3000000),
        (3, 'محاسب', 'محاسب مالي', 3, 800000, 1500000),
        (4, 'مهندس برمجيات', 'مطور برامج وتطبيقات', 4, 1200000, 2500000),
        (5, 'مندوب مبيعات', 'مسؤول المبيعات والتسويق', 5, 600000, 1200000);

        -- إدخال 5 موظفين
        INSERT OR IGNORE INTO Employees (Id, EmployeeNumber, FirstName, LastName, Email, Phone, DepartmentId, PositionId, HireDate, BasicSalary, Gender, Address, City) VALUES
        (1, 'EMP001', 'أحمد', 'الموسوي', 'ahmed@company.iq', '07701234567', 1, 1, '2020-01-15', 3500000, 'Male', 'شارع الرشيد', 'بغداد'),
        (2, 'EMP002', 'فاطمة', 'العلي', 'fatima@company.iq', '07702345678', 2, 2, '2021-03-01', 2200000, 'Female', 'المنصور', 'بغداد'),
        (3, 'EMP003', 'محمد', 'الحسيني', 'mohammed@company.iq', '07703456789', 3, 3, '2022-06-15', 1200000, 'Male', 'الكرادة', 'بغداد'),
        (4, 'EMP004', 'زينب', 'الكاظمي', 'zainab@company.iq', '07704567890', 4, 4, '2024-01-01', 1800000, 'Female', 'الجادرية', 'بغداد'),
        (5, 'EMP005', 'علي', 'الربيعي', 'ali@company.iq', '07705678901', 5, 5, '2023-01-01', 900000, 'Male', 'الكاظمية', 'بغداد');

        -- إدخال مستخدم مدير (كلمة المرور: admin123)
        INSERT OR IGNORE INTO Users (Id, Username, PasswordHash, EmployeeId, Role) VALUES
        (1, 'admin', '$2a$11$OgmPvXHAbKxbflVy8VjyJuQ0zNV.1PQ791LoqLezErsmE.y3fKkOa', 1, 'Admin');

        -- إدخال 5 سجلات حضور
        INSERT OR IGNORE INTO Attendance (Id, EmployeeId, Date, CheckInTime, CheckOutTime, WorkHours, Status) VALUES
        (1, 1, date('now'), '08:00', '16:00', 8, 'Present'),
        (2, 2, date('now'), '08:30', '16:30', 8, 'Present'),
        (3, 3, date('now'), '09:00', '17:00', 8, 'Present'),
        (4, 4, date('now'), '08:15', '16:15', 8, 'Present'),
        (5, 5, date('now'), NULL, NULL, 0, 'Absent');

        -- إدخال 5 طلبات إجازة
        INSERT OR IGNORE INTO Leaves (Id, EmployeeId, LeaveTypeId, StartDate, EndDate, TotalDays, Reason, Status) VALUES
        (1, 2, 1, date('now', '+7 days'), date('now', '+14 days'), 7, 'إجازة سنوية للسفر', 'Pending'),
        (2, 3, 2, date('now', '-3 days'), date('now', '-1 days'), 3, 'مرض عارض', 'Approved'),
        (3, 4, 1, date('now', '+30 days'), date('now', '+35 days'), 5, 'زيارة عائلية', 'Pending'),
        (4, 5, 4, date('now', '+60 days'), date('now', '+65 days'), 5, 'إجازة زواج', 'Approved'),
        (5, 1, 3, date('now', '+90 days'), date('now', '+100 days'), 10, 'سفر للخارج', 'Pending');

        -- إدخال 5 سجلات رواتب
        INSERT OR IGNORE INTO Payroll (Id, EmployeeId, Month, Year, BasicSalary, TotalAllowances, TotalDeductions, GrossSalary, NetSalary, Status) VALUES
        (1, 1, 11, 2025, 3500000, 500000, 200000, 4000000, 3800000, 'Paid'),
        (2, 2, 11, 2025, 2200000, 300000, 150000, 2500000, 2350000, 'Paid'),
        (3, 3, 11, 2025, 1200000, 200000, 100000, 1400000, 1300000, 'Processed'),
        (4, 4, 11, 2025, 1800000, 250000, 120000, 2050000, 1930000, 'Draft'),
        (5, 5, 11, 2025, 900000, 150000, 80000, 1050000, 970000, 'Draft');

        -- إدخال أرصدة إجازات للموظفين
        INSERT OR IGNORE INTO LeaveBalances (Id, EmployeeId, LeaveTypeId, Year, TotalDays, UsedDays, RemainingDays) VALUES
        (1, 1, 1, 2025, 21, 5, 16),
        (2, 2, 1, 2025, 21, 7, 14),
        (3, 3, 1, 2025, 21, 3, 18),
        (4, 4, 1, 2025, 21, 0, 21),
        (5, 5, 1, 2025, 21, 10, 11);
    ";
    #endregion
}

