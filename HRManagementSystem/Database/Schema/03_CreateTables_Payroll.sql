-- =====================================================
-- جدول الرواتب - Payroll Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Payroll (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    PayrollMonth INTEGER NOT NULL,                -- شهر الراتب (1-12)
    PayrollYear INTEGER NOT NULL,                 -- سنة الراتب
    BasicSalary REAL NOT NULL DEFAULT 0,          -- الراتب الأساسي
    TotalAllowances REAL DEFAULT 0,               -- إجمالي البدلات
    TotalDeductions REAL DEFAULT 0,               -- إجمالي الخصومات
    OvertimeAmount REAL DEFAULT 0,                -- مبلغ العمل الإضافي
    OvertimeHours REAL DEFAULT 0,                 -- ساعات العمل الإضافي
    GrossSalary REAL DEFAULT 0,                   -- إجمالي الراتب
    NetSalary REAL DEFAULT 0,                     -- صافي الراتب
    Status TEXT DEFAULT 'مسودة' CHECK(Status IN ('مسودة', 'معتمد', 'مدفوع', 'ملغى')),
    PaymentDate TEXT,                             -- تاريخ الدفع
    PaymentMethod TEXT,                           -- طريقة الدفع
    Notes TEXT,                                   -- ملاحظات
    ApprovedBy INTEGER,                           -- المعتمد
    ApprovedAt TEXT,                              -- تاريخ الاعتماد
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (ApprovedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    UNIQUE(EmployeeId, PayrollMonth, PayrollYear)
);

-- =====================================================
-- جدول تفاصيل الرواتب - PayrollDetails Table
-- =====================================================
CREATE TABLE IF NOT EXISTS PayrollDetails (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PayrollId INTEGER NOT NULL,                   -- الراتب
    ItemType TEXT NOT NULL CHECK(ItemType IN ('بدل', 'خصم')),  -- نوع البند
    ItemName TEXT NOT NULL,                       -- اسم البند
    Amount REAL NOT NULL DEFAULT 0,               -- المبلغ
    Percentage REAL,                              -- النسبة المئوية
    IsFixed INTEGER DEFAULT 1,                    -- ثابت أم متغير
    Notes TEXT,                                   -- ملاحظات
    CreatedAt TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (PayrollId) REFERENCES Payroll(Id) ON DELETE CASCADE
);

-- =====================================================
-- جدول عناصر الراتب الثابتة - SalaryComponents Table
-- =====================================================
CREATE TABLE IF NOT EXISTS SalaryComponents (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,                           -- اسم العنصر
    NameEn TEXT,                                  -- Component Name
    Type TEXT NOT NULL CHECK(Type IN ('بدل', 'خصم')),  -- نوع (بدل/خصم)
    CalculationType TEXT CHECK(CalculationType IN ('مبلغ ثابت', 'نسبة من الراتب')),
    DefaultAmount REAL DEFAULT 0,                 -- المبلغ الافتراضي
    DefaultPercentage REAL DEFAULT 0,             -- النسبة الافتراضية
    IsTaxable INTEGER DEFAULT 0,                  -- خاضع للضريبة
    IsActive INTEGER DEFAULT 1,
    SortOrder INTEGER DEFAULT 0,
    CreatedAt TEXT DEFAULT (datetime('now'))
);

-- =====================================================
-- جدول عناصر راتب الموظف - EmployeeSalaryComponents Table
-- =====================================================
CREATE TABLE IF NOT EXISTS EmployeeSalaryComponents (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    SalaryComponentId INTEGER NOT NULL,           -- عنصر الراتب
    Amount REAL DEFAULT 0,                        -- المبلغ
    Percentage REAL DEFAULT 0,                    -- النسبة
    EffectiveDate TEXT,                           -- تاريخ السريان
    EndDate TEXT,                                 -- تاريخ الانتهاء
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (SalaryComponentId) REFERENCES SalaryComponents(Id) ON DELETE CASCADE
);

-- =====================================================
-- جدول المستندات - Documents Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Documents (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    DocumentType TEXT NOT NULL,                   -- نوع المستند
    DocumentName TEXT NOT NULL,                   -- اسم المستند
    FileName TEXT NOT NULL,                       -- اسم الملف
    FilePath TEXT NOT NULL,                       -- مسار الملف
    FileSize INTEGER,                             -- حجم الملف
    MimeType TEXT,                                -- نوع الملف
    ExpiryDate TEXT,                              -- تاريخ الانتهاء
    Notes TEXT,                                   -- ملاحظات
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);

-- =====================================================
-- جدول سجل النظام - AuditLog Table
-- =====================================================
CREATE TABLE IF NOT EXISTS AuditLog (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,                               -- المستخدم
    Action TEXT NOT NULL,                         -- الإجراء
    TableName TEXT,                               -- الجدول
    RecordId INTEGER,                             -- معرف السجل
    OldValues TEXT,                               -- القيم القديمة (JSON)
    NewValues TEXT,                               -- القيم الجديدة (JSON)
    IpAddress TEXT,                               -- عنوان IP
    UserAgent TEXT,                               -- المتصفح
    CreatedAt TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- =====================================================
-- جدول إعدادات النظام - SystemSettings Table
-- =====================================================
CREATE TABLE IF NOT EXISTS SystemSettings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SettingKey TEXT NOT NULL UNIQUE,              -- مفتاح الإعداد
    SettingValue TEXT,                            -- قيمة الإعداد
    SettingType TEXT DEFAULT 'string',            -- نوع الإعداد
    Description TEXT,                             -- الوصف
    UpdatedAt TEXT DEFAULT (datetime('now')),
    UpdatedBy INTEGER
);

