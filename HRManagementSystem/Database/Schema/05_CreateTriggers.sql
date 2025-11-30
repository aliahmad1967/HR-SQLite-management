-- =====================================================
-- المشغلات - Triggers
-- =====================================================

-- مشغل تحديث تاريخ التعديل للموظفين
CREATE TRIGGER IF NOT EXISTS trg_employees_update_timestamp
AFTER UPDATE ON Employees
BEGIN
    UPDATE Employees SET UpdatedAt = datetime('now') WHERE Id = NEW.Id;
END;

-- مشغل تحديث تاريخ التعديل للأقسام
CREATE TRIGGER IF NOT EXISTS trg_departments_update_timestamp
AFTER UPDATE ON Departments
BEGIN
    UPDATE Departments SET UpdatedAt = datetime('now') WHERE Id = NEW.Id;
END;

-- مشغل تحديث تاريخ التعديل للمستخدمين
CREATE TRIGGER IF NOT EXISTS trg_users_update_timestamp
AFTER UPDATE ON Users
BEGIN
    UPDATE Users SET UpdatedAt = datetime('now') WHERE Id = NEW.Id;
END;

-- مشغل تحديث تاريخ التعديل للحضور
CREATE TRIGGER IF NOT EXISTS trg_attendance_update_timestamp
AFTER UPDATE ON Attendance
BEGIN
    UPDATE Attendance SET UpdatedAt = datetime('now') WHERE Id = NEW.Id;
END;

-- مشغل حساب ساعات العمل تلقائياً
CREATE TRIGGER IF NOT EXISTS trg_attendance_calculate_hours
AFTER UPDATE OF CheckOutTime ON Attendance
WHEN NEW.CheckOutTime IS NOT NULL AND NEW.CheckInTime IS NOT NULL
BEGIN
    UPDATE Attendance 
    SET WorkingHours = ROUND(
        (julianday(NEW.CheckOutTime) - julianday(NEW.CheckInTime)) * 24, 2
    )
    WHERE Id = NEW.Id;
END;

-- مشغل تحديث رصيد الإجازات عند الموافقة
CREATE TRIGGER IF NOT EXISTS trg_leaves_update_balance
AFTER UPDATE OF Status ON Leaves
WHEN NEW.Status = 'موافق عليها' AND OLD.Status != 'موافق عليها'
BEGIN
    UPDATE LeaveBalances 
    SET UsedDays = UsedDays + NEW.TotalDays,
        RemainingDays = TotalDays + CarriedOverDays - (UsedDays + NEW.TotalDays),
        UpdatedAt = datetime('now')
    WHERE EmployeeId = NEW.EmployeeId 
    AND LeaveTypeId = NEW.LeaveTypeId 
    AND Year = CAST(strftime('%Y', NEW.StartDate) AS INTEGER);
END;

-- مشغل إرجاع رصيد الإجازات عند الإلغاء
CREATE TRIGGER IF NOT EXISTS trg_leaves_restore_balance
AFTER UPDATE OF Status ON Leaves
WHEN NEW.Status = 'ملغاة' AND OLD.Status = 'موافق عليها'
BEGIN
    UPDATE LeaveBalances 
    SET UsedDays = UsedDays - NEW.TotalDays,
        RemainingDays = TotalDays + CarriedOverDays - (UsedDays - NEW.TotalDays),
        UpdatedAt = datetime('now')
    WHERE EmployeeId = NEW.EmployeeId 
    AND LeaveTypeId = NEW.LeaveTypeId 
    AND Year = CAST(strftime('%Y', NEW.StartDate) AS INTEGER);
END;

-- مشغل حساب صافي الراتب
CREATE TRIGGER IF NOT EXISTS trg_payroll_calculate_net
AFTER UPDATE OF TotalAllowances, TotalDeductions, OvertimeAmount ON Payroll
BEGIN
    UPDATE Payroll 
    SET GrossSalary = BasicSalary + TotalAllowances + OvertimeAmount,
        NetSalary = BasicSalary + TotalAllowances + OvertimeAmount - TotalDeductions,
        UpdatedAt = datetime('now')
    WHERE Id = NEW.Id;
END;

-- مشغل إنشاء رصيد إجازات للموظف الجديد
CREATE TRIGGER IF NOT EXISTS trg_employee_create_leave_balances
AFTER INSERT ON Employees
BEGIN
    INSERT INTO LeaveBalances (EmployeeId, LeaveTypeId, Year, TotalDays, UsedDays, RemainingDays)
    SELECT NEW.Id, lt.Id, CAST(strftime('%Y', 'now') AS INTEGER), lt.DefaultDays, 0, lt.DefaultDays
    FROM LeaveTypes lt WHERE lt.IsActive = 1;
END;

-- مشغل تسجيل تاريخ التوظيف عند إضافة موظف
CREATE TRIGGER IF NOT EXISTS trg_employee_insert_history
AFTER INSERT ON Employees
BEGIN
    INSERT INTO EmploymentHistory (
        EmployeeId, ActionType, ActionDate, ToDepartmentId, ToPositionId, ToSalary, Notes, CreatedBy
    ) VALUES (
        NEW.Id, 'تعيين', NEW.HireDate, NEW.DepartmentId, NEW.PositionId, NEW.BasicSalary, 'تعيين موظف جديد', NEW.CreatedBy
    );
END;

-- مشغل تسجيل التغييرات في القسم أو المسمى
CREATE TRIGGER IF NOT EXISTS trg_employee_position_change
AFTER UPDATE OF DepartmentId, PositionId, BasicSalary ON Employees
WHEN OLD.DepartmentId != NEW.DepartmentId OR OLD.PositionId != NEW.PositionId OR OLD.BasicSalary != NEW.BasicSalary
BEGIN
    INSERT INTO EmploymentHistory (
        EmployeeId, ActionType, ActionDate, 
        FromDepartmentId, ToDepartmentId, 
        FromPositionId, ToPositionId, 
        FromSalary, ToSalary, 
        Notes, CreatedBy
    ) VALUES (
        NEW.Id, 
        CASE 
            WHEN OLD.PositionId != NEW.PositionId THEN 'ترقية/نقل'
            WHEN OLD.DepartmentId != NEW.DepartmentId THEN 'نقل'
            WHEN OLD.BasicSalary != NEW.BasicSalary THEN 'تعديل راتب'
            ELSE 'تحديث'
        END,
        datetime('now'), 
        OLD.DepartmentId, NEW.DepartmentId, 
        OLD.PositionId, NEW.PositionId, 
        OLD.BasicSalary, NEW.BasicSalary, 
        'تحديث بيانات الموظف', NEW.UpdatedBy
    );
END;

-- مشغل توليد رقم الموظف تلقائياً
CREATE TRIGGER IF NOT EXISTS trg_employee_generate_number
AFTER INSERT ON Employees
WHEN NEW.EmployeeNumber IS NULL OR NEW.EmployeeNumber = ''
BEGIN
    UPDATE Employees 
    SET EmployeeNumber = 'EMP' || printf('%06d', NEW.Id)
    WHERE Id = NEW.Id;
END;

