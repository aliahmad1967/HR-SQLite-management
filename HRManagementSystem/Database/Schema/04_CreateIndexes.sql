-- =====================================================
-- الفهارس - Indexes
-- استراتيجية الفهرسة لتحسين الأداء
-- =====================================================

-- فهارس جدول الموظفين
CREATE INDEX IF NOT EXISTS idx_employees_employee_number ON Employees(EmployeeNumber);
CREATE INDEX IF NOT EXISTS idx_employees_national_id ON Employees(NationalId);
CREATE INDEX IF NOT EXISTS idx_employees_department ON Employees(DepartmentId);
CREATE INDEX IF NOT EXISTS idx_employees_position ON Employees(PositionId);
CREATE INDEX IF NOT EXISTS idx_employees_manager ON Employees(ManagerId);
CREATE INDEX IF NOT EXISTS idx_employees_status ON Employees(EmploymentStatus);
CREATE INDEX IF NOT EXISTS idx_employees_hire_date ON Employees(HireDate);
CREATE INDEX IF NOT EXISTS idx_employees_active ON Employees(IsActive);
CREATE INDEX IF NOT EXISTS idx_employees_name ON Employees(FirstName, LastName);

-- فهارس جدول الأقسام
CREATE INDEX IF NOT EXISTS idx_departments_name ON Departments(Name);
CREATE INDEX IF NOT EXISTS idx_departments_parent ON Departments(ParentDepartmentId);
CREATE INDEX IF NOT EXISTS idx_departments_active ON Departments(IsActive);

-- فهارس جدول المسميات الوظيفية
CREATE INDEX IF NOT EXISTS idx_positions_title ON Positions(Title);
CREATE INDEX IF NOT EXISTS idx_positions_department ON Positions(DepartmentId);
CREATE INDEX IF NOT EXISTS idx_positions_active ON Positions(IsActive);

-- فهارس جدول المستخدمين
CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username);
CREATE INDEX IF NOT EXISTS idx_users_employee ON Users(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_users_role ON Users(Role);
CREATE INDEX IF NOT EXISTS idx_users_active ON Users(IsActive);

-- فهارس جدول الحضور
CREATE INDEX IF NOT EXISTS idx_attendance_employee ON Attendance(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_attendance_date ON Attendance(AttendanceDate);
CREATE INDEX IF NOT EXISTS idx_attendance_status ON Attendance(Status);
CREATE INDEX IF NOT EXISTS idx_attendance_employee_date ON Attendance(EmployeeId, AttendanceDate);

-- فهارس جدول الإجازات
CREATE INDEX IF NOT EXISTS idx_leaves_employee ON Leaves(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_leaves_type ON Leaves(LeaveTypeId);
CREATE INDEX IF NOT EXISTS idx_leaves_status ON Leaves(Status);
CREATE INDEX IF NOT EXISTS idx_leaves_dates ON Leaves(StartDate, EndDate);
CREATE INDEX IF NOT EXISTS idx_leaves_approved_by ON Leaves(ApprovedBy);

-- فهارس جدول أرصدة الإجازات
CREATE INDEX IF NOT EXISTS idx_leave_balances_employee ON LeaveBalances(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_leave_balances_type ON LeaveBalances(LeaveTypeId);
CREATE INDEX IF NOT EXISTS idx_leave_balances_year ON LeaveBalances(Year);

-- فهارس جدول الرواتب
CREATE INDEX IF NOT EXISTS idx_payroll_employee ON Payroll(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_payroll_period ON Payroll(PayrollYear, PayrollMonth);
CREATE INDEX IF NOT EXISTS idx_payroll_status ON Payroll(Status);
CREATE INDEX IF NOT EXISTS idx_payroll_payment_date ON Payroll(PaymentDate);

-- فهارس جدول تفاصيل الرواتب
CREATE INDEX IF NOT EXISTS idx_payroll_details_payroll ON PayrollDetails(PayrollId);
CREATE INDEX IF NOT EXISTS idx_payroll_details_type ON PayrollDetails(ItemType);

-- فهارس جدول المستندات
CREATE INDEX IF NOT EXISTS idx_documents_employee ON Documents(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_documents_type ON Documents(DocumentType);
CREATE INDEX IF NOT EXISTS idx_documents_expiry ON Documents(ExpiryDate);
CREATE INDEX IF NOT EXISTS idx_documents_active ON Documents(IsActive);

-- فهارس جدول تاريخ التوظيف
CREATE INDEX IF NOT EXISTS idx_employment_history_employee ON EmploymentHistory(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_employment_history_date ON EmploymentHistory(ActionDate);
CREATE INDEX IF NOT EXISTS idx_employment_history_type ON EmploymentHistory(ActionType);

-- فهارس جدول سجل النظام
CREATE INDEX IF NOT EXISTS idx_audit_log_user ON AuditLog(UserId);
CREATE INDEX IF NOT EXISTS idx_audit_log_table ON AuditLog(TableName);
CREATE INDEX IF NOT EXISTS idx_audit_log_date ON AuditLog(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_audit_log_action ON AuditLog(Action);

-- فهارس جدول عناصر الراتب
CREATE INDEX IF NOT EXISTS idx_salary_components_type ON SalaryComponents(Type);
CREATE INDEX IF NOT EXISTS idx_salary_components_active ON SalaryComponents(IsActive);

-- فهارس جدول عناصر راتب الموظف
CREATE INDEX IF NOT EXISTS idx_emp_salary_comp_employee ON EmployeeSalaryComponents(EmployeeId);
CREATE INDEX IF NOT EXISTS idx_emp_salary_comp_component ON EmployeeSalaryComponents(SalaryComponentId);
CREATE INDEX IF NOT EXISTS idx_emp_salary_comp_active ON EmployeeSalaryComponents(IsActive);

