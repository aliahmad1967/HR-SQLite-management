-- =====================================================
-- جدول الحضور - Attendance Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Attendance (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    AttendanceDate TEXT NOT NULL,                 -- تاريخ الحضور
    CheckInTime TEXT,                             -- وقت الحضور
    CheckOutTime TEXT,                            -- وقت الانصراف
    Status TEXT DEFAULT 'حاضر' CHECK(Status IN ('حاضر', 'غائب', 'متأخر', 'إجازة', 'مهمة عمل')),
    WorkingHours REAL,                            -- ساعات العمل
    OvertimeHours REAL DEFAULT 0,                 -- ساعات إضافية
    Notes TEXT,                                   -- ملاحظات
    IsManualEntry INTEGER DEFAULT 0,              -- إدخال يدوي
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    UNIQUE(EmployeeId, AttendanceDate)
);

-- =====================================================
-- جدول أنواع الإجازات - LeaveTypes Table
-- =====================================================
CREATE TABLE IF NOT EXISTS LeaveTypes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,                    -- نوع الإجازة
    NameEn TEXT,                                  -- Leave Type (English)
    DefaultDays INTEGER DEFAULT 0,                -- الأيام الافتراضية
    IsPaid INTEGER DEFAULT 1,                     -- مدفوعة
    RequiresApproval INTEGER DEFAULT 1,           -- تتطلب موافقة
    Color TEXT DEFAULT '#4CAF50',                 -- لون العرض
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT (datetime('now'))
);

-- =====================================================
-- جدول الإجازات - Leaves Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Leaves (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    LeaveTypeId INTEGER NOT NULL,                 -- نوع الإجازة
    StartDate TEXT NOT NULL,                      -- تاريخ البداية
    EndDate TEXT NOT NULL,                        -- تاريخ النهاية
    TotalDays INTEGER NOT NULL,                   -- إجمالي الأيام
    Reason TEXT,                                  -- السبب
    Status TEXT DEFAULT 'قيد الانتظار' CHECK(Status IN ('قيد الانتظار', 'موافق عليها', 'مرفوضة', 'ملغاة')),
    ApprovedBy INTEGER,                           -- المعتمد
    ApprovedAt TEXT,                              -- تاريخ الاعتماد
    RejectionReason TEXT,                         -- سبب الرفض
    AttachmentPath TEXT,                          -- مرفق
    Notes TEXT,                                   -- ملاحظات
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ApprovedBy) REFERENCES Users(Id) ON DELETE SET NULL
);

-- =====================================================
-- جدول أرصدة الإجازات - LeaveBalances Table
-- =====================================================
CREATE TABLE IF NOT EXISTS LeaveBalances (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    LeaveTypeId INTEGER NOT NULL,                 -- نوع الإجازة
    Year INTEGER NOT NULL,                        -- السنة
    TotalDays REAL DEFAULT 0,                     -- إجمالي الأيام
    UsedDays REAL DEFAULT 0,                      -- الأيام المستخدمة
    RemainingDays REAL DEFAULT 0,                 -- الأيام المتبقية
    CarriedOverDays REAL DEFAULT 0,               -- أيام مرحلة
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id) ON DELETE CASCADE,
    UNIQUE(EmployeeId, LeaveTypeId, Year)
);

-- =====================================================
-- جدول تاريخ التوظيف - EmploymentHistory Table
-- =====================================================
CREATE TABLE IF NOT EXISTS EmploymentHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,                  -- الموظف
    ActionType TEXT NOT NULL,                     -- نوع الإجراء (تعيين، ترقية، نقل، إنهاء)
    ActionDate TEXT NOT NULL,                     -- تاريخ الإجراء
    FromDepartmentId INTEGER,                     -- من القسم
    ToDepartmentId INTEGER,                       -- إلى القسم
    FromPositionId INTEGER,                       -- من المسمى
    ToPositionId INTEGER,                         -- إلى المسمى
    FromSalary REAL,                              -- الراتب السابق
    ToSalary REAL,                                -- الراتب الجديد
    Reason TEXT,                                  -- السبب
    Notes TEXT,                                   -- ملاحظات
    ApprovedBy INTEGER,                           -- المعتمد
    CreatedAt TEXT DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (FromDepartmentId) REFERENCES Departments(Id) ON DELETE SET NULL,
    FOREIGN KEY (ToDepartmentId) REFERENCES Departments(Id) ON DELETE SET NULL,
    FOREIGN KEY (FromPositionId) REFERENCES Positions(Id) ON DELETE SET NULL,
    FOREIGN KEY (ToPositionId) REFERENCES Positions(Id) ON DELETE SET NULL
);

