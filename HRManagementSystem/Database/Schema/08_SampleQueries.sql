-- =====================================================
-- استعلامات نموذجية - Sample Queries
-- =====================================================

-- ================== CRUD للموظفين ==================

-- إضافة موظف جديد
INSERT INTO Employees (
    FirstName, SecondName, ThirdName, LastName, NationalId, Gender, 
    DateOfBirth, Mobile, Email, DepartmentId, PositionId, HireDate, BasicSalary
) VALUES (
    'محمد', 'أحمد', 'علي', 'السعيد', '1234567890', 'ذكر',
    '1990-05-15', '0501234567', 'mohammed@example.com', 2, 3, date('now'), 8000
);

-- تحديث بيانات موظف
UPDATE Employees SET 
    Mobile = '0509876543',
    Email = 'new.email@example.com',
    BasicSalary = 9000,
    UpdatedBy = 1
WHERE Id = 1;

-- حذف موظف (تعطيل)
UPDATE Employees SET IsActive = 0, EmploymentStatus = 'منتهي' WHERE Id = 1;

-- البحث عن موظف بالاسم
SELECT * FROM vw_EmployeesFullInfo 
WHERE الاسم_الكامل LIKE '%محمد%';

-- جلب جميع الموظفين النشطين
SELECT * FROM Employees WHERE IsActive = 1 AND EmploymentStatus = 'نشط';

-- ================== CRUD للحضور ==================

-- تسجيل حضور
INSERT INTO Attendance (EmployeeId, AttendanceDate, CheckInTime, Status, CreatedBy)
VALUES (1, date('now'), time('now'), 'حاضر', 1);

-- تسجيل انصراف
UPDATE Attendance SET CheckOutTime = time('now')
WHERE EmployeeId = 1 AND AttendanceDate = date('now');

-- تقرير حضور يومي
SELECT 
    e.EmployeeNumber, 
    e.FirstName || ' ' || e.LastName AS الاسم,
    a.CheckInTime AS الحضور,
    a.CheckOutTime AS الانصراف,
    a.WorkingHours AS ساعات_العمل,
    a.Status AS الحالة
FROM Attendance a
JOIN Employees e ON a.EmployeeId = e.Id
WHERE a.AttendanceDate = date('now');

-- تقرير حضور شهري
SELECT * FROM vw_MonthlyAttendanceStats
WHERE الشهر = strftime('%Y-%m', 'now');

-- ================== CRUD للإجازات ==================

-- طلب إجازة
INSERT INTO Leaves (EmployeeId, LeaveTypeId, StartDate, EndDate, TotalDays, Reason, CreatedBy)
VALUES (1, 1, '2024-01-15', '2024-01-20', 5, 'إجازة عائلية', 1);

-- الموافقة على إجازة
UPDATE Leaves SET 
    Status = 'موافق عليها',
    ApprovedBy = 1,
    ApprovedAt = datetime('now')
WHERE Id = 1;

-- رفض إجازة
UPDATE Leaves SET 
    Status = 'مرفوضة',
    RejectionReason = 'ضغط العمل في هذه الفترة',
    ApprovedBy = 1,
    ApprovedAt = datetime('now')
WHERE Id = 1;

-- عرض أرصدة إجازات موظف
SELECT * FROM vw_CurrentLeaveBalances WHERE EmployeeId = 1;

-- ================== CRUD للرواتب ==================

-- إنشاء راتب شهري
INSERT INTO Payroll (EmployeeId, PayrollMonth, PayrollYear, BasicSalary, CreatedBy)
SELECT Id, 1, 2024, BasicSalary, 1 FROM Employees WHERE IsActive = 1;

-- إضافة بدل للراتب
INSERT INTO PayrollDetails (PayrollId, ItemType, ItemName, Amount)
VALUES (1, 'بدل', 'بدل سكن', 2000);

-- إضافة خصم للراتب
INSERT INTO PayrollDetails (PayrollId, ItemType, ItemName, Amount)
VALUES (1, 'خصم', 'تأمينات', 780);

-- حساب إجماليات الراتب
UPDATE Payroll SET 
    TotalAllowances = (SELECT COALESCE(SUM(Amount), 0) FROM PayrollDetails WHERE PayrollId = 1 AND ItemType = 'بدل'),
    TotalDeductions = (SELECT COALESCE(SUM(Amount), 0) FROM PayrollDetails WHERE PayrollId = 1 AND ItemType = 'خصم')
WHERE Id = 1;

-- اعتماد الرواتب
UPDATE Payroll SET Status = 'معتمد', ApprovedBy = 1, ApprovedAt = datetime('now')
WHERE PayrollMonth = 1 AND PayrollYear = 2024;

-- ================== تقارير متقدمة ==================

-- إحصائيات لوحة التحكم
SELECT 
    (SELECT COUNT(*) FROM Employees WHERE IsActive = 1) AS إجمالي_الموظفين,
    (SELECT COUNT(*) FROM Employees WHERE IsActive = 1 AND EmploymentStatus = 'نشط') AS الموظفين_النشطين,
    (SELECT COUNT(*) FROM Departments WHERE IsActive = 1) AS عدد_الأقسام,
    (SELECT COUNT(*) FROM Leaves WHERE Status = 'قيد الانتظار') AS الإجازات_المعلقة,
    (SELECT SUM(NetSalary) FROM Payroll WHERE PayrollYear = strftime('%Y', 'now') AND PayrollMonth = strftime('%m', 'now')) AS إجمالي_الرواتب_الشهرية;

-- توزيع الموظفين حسب القسم
SELECT d.Name AS القسم, COUNT(e.Id) AS العدد
FROM Departments d
LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsActive = 1
GROUP BY d.Id ORDER BY العدد DESC;

-- أكثر 10 موظفين غياباً
SELECT e.EmployeeNumber, e.FirstName || ' ' || e.LastName AS الاسم, COUNT(a.Id) AS أيام_الغياب
FROM Employees e
JOIN Attendance a ON e.Id = a.EmployeeId AND a.Status = 'غائب'
WHERE a.AttendanceDate >= date('now', '-30 days')
GROUP BY e.Id ORDER BY أيام_الغياب DESC LIMIT 10;

