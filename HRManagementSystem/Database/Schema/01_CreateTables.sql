-- =====================================================
-- نظام إدارة الموارد البشرية - HR Management System
-- Database Schema - SQLite
-- Version: 1.0.0
-- =====================================================

-- Enable foreign keys
PRAGMA foreign_keys = ON;

-- =====================================================
-- جدول الأقسام - Departments Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Departments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,                    -- اسم القسم
    NameEn TEXT,                                  -- Department Name (English)
    Description TEXT,                             -- وصف القسم
    ManagerId INTEGER,                            -- مدير القسم
    ParentDepartmentId INTEGER,                   -- القسم الأب (للهيكل الهرمي)
    IsActive INTEGER DEFAULT 1,                   -- حالة النشاط
    CreatedAt TEXT DEFAULT (datetime('now')),     -- تاريخ الإنشاء
    UpdatedAt TEXT DEFAULT (datetime('now')),     -- تاريخ التحديث
    FOREIGN KEY (ParentDepartmentId) REFERENCES Departments(Id) ON DELETE SET NULL
);

-- =====================================================
-- جدول المسميات الوظيفية - Positions Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Positions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,                          -- المسمى الوظيفي
    TitleEn TEXT,                                 -- Position Title (English)
    DepartmentId INTEGER NOT NULL,                -- القسم
    Description TEXT,                             -- وصف الوظيفة
    MinSalary REAL DEFAULT 0,                     -- الحد الأدنى للراتب
    MaxSalary REAL DEFAULT 0,                     -- الحد الأعلى للراتب
    IsActive INTEGER DEFAULT 1,                   -- حالة النشاط
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id) ON DELETE RESTRICT
);

-- =====================================================
-- جدول الموظفين - Employees Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Employees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeNumber TEXT NOT NULL UNIQUE,          -- رقم الموظف (تلقائي)
    FirstName TEXT NOT NULL,                      -- الاسم الأول
    SecondName TEXT,                              -- الاسم الثاني
    ThirdName TEXT,                               -- الاسم الثالث
    LastName TEXT NOT NULL,                       -- اسم العائلة
    FullNameAr TEXT,                              -- الاسم الكامل بالعربي
    FullNameEn TEXT,                              -- Full Name (English)
    NationalId TEXT UNIQUE,                       -- رقم الهوية الوطنية
    Nationality TEXT,                             -- الجنسية
    Gender TEXT CHECK(Gender IN ('ذكر', 'أنثى')), -- الجنس
    DateOfBirth TEXT,                             -- تاريخ الميلاد
    PlaceOfBirth TEXT,                            -- مكان الميلاد
    MaritalStatus TEXT,                           -- الحالة الاجتماعية
    Email TEXT,                                   -- البريد الإلكتروني
    Phone TEXT,                                   -- رقم الهاتف
    Mobile TEXT,                                  -- رقم الجوال
    Address TEXT,                                 -- العنوان
    City TEXT,                                    -- المدينة
    PostalCode TEXT,                              -- الرمز البريدي
    EmergencyContactName TEXT,                    -- اسم جهة الاتصال للطوارئ
    EmergencyContactPhone TEXT,                   -- رقم جهة الاتصال للطوارئ
    DepartmentId INTEGER,                         -- القسم
    PositionId INTEGER,                           -- المسمى الوظيفي
    ManagerId INTEGER,                            -- المدير المباشر
    HireDate TEXT NOT NULL,                       -- تاريخ التعيين
    ContractType TEXT,                            -- نوع العقد
    ContractEndDate TEXT,                         -- تاريخ انتهاء العقد
    EmploymentStatus TEXT DEFAULT 'نشط',          -- حالة التوظيف
    BasicSalary REAL DEFAULT 0,                   -- الراتب الأساسي
    BankName TEXT,                                -- اسم البنك
    BankAccountNumber TEXT,                       -- رقم الحساب البنكي
    IBAN TEXT,                                    -- رقم الآيبان
    ProfilePhoto TEXT,                            -- صورة الموظف (مسار)
    Notes TEXT,                                   -- ملاحظات
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    UpdatedBy INTEGER,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id) ON DELETE SET NULL,
    FOREIGN KEY (PositionId) REFERENCES Positions(Id) ON DELETE SET NULL,
    FOREIGN KEY (ManagerId) REFERENCES Employees(Id) ON DELETE SET NULL
);

-- =====================================================
-- جدول المستخدمين - Users Table
-- =====================================================
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,                -- اسم المستخدم
    PasswordHash TEXT NOT NULL,                   -- كلمة المرور المشفرة
    Email TEXT UNIQUE,                            -- البريد الإلكتروني
    EmployeeId INTEGER,                           -- ربط بالموظف
    Role TEXT NOT NULL CHECK(Role IN ('مدير النظام', 'مدير الموارد البشرية', 'مستخدم')),
    IsActive INTEGER DEFAULT 1,
    LastLoginAt TEXT,                             -- آخر تسجيل دخول
    FailedLoginAttempts INTEGER DEFAULT 0,        -- محاولات الدخول الفاشلة
    LockedUntil TEXT,                             -- مقفل حتى
    CreatedAt TEXT DEFAULT (datetime('now')),
    UpdatedAt TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE SET NULL
);

