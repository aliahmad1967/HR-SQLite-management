-- =====================================================
-- العروض - Views
-- =====================================================

-- عرض بيانات الموظفين الكاملة
CREATE VIEW IF NOT EXISTS vw_EmployeesFullInfo AS
SELECT 
    e.Id,
    e.EmployeeNumber AS رقم_الموظف,
    e.FirstName || ' ' || COALESCE(e.SecondName, '') || ' ' || COALESCE(e.ThirdName, '') || ' ' || e.LastName AS الاسم_الكامل,
    e.NationalId AS رقم_الهوية,
    e.Gender AS الجنس,
    e.DateOfBirth AS تاريخ_الميلاد,
    e.Mobile AS الجوال,
    e.Email AS البريد_الإلكتروني,
    d.Name AS القسم,
    p.Title AS المسمى_الوظيفي,
    m.FirstName || ' ' || m.LastName AS المدير_المباشر,
    e.HireDate AS تاريخ_التعيين,
    e.BasicSalary AS الراتب_الأساسي,
    e.EmploymentStatus AS حالة_التوظيف,
    e.ContractType AS نوع_العقد,
    e.IsActive AS نشط
FROM Employees e
LEFT JOIN Departments d ON e.DepartmentId = d.Id
LEFT JOIN Positions p ON e.PositionId = p.Id
LEFT JOIN Employees m ON e.ManagerId = m.Id;

-- عرض إحصائيات الحضور الشهري
CREATE VIEW IF NOT EXISTS vw_MonthlyAttendanceStats AS
SELECT 
    e.Id AS EmployeeId,
    e.EmployeeNumber AS رقم_الموظف,
    e.FirstName || ' ' || e.LastName AS اسم_الموظف,
    d.Name AS القسم,
    strftime('%Y-%m', a.AttendanceDate) AS الشهر,
    COUNT(CASE WHEN a.Status = 'حاضر' THEN 1 END) AS أيام_الحضور,
    COUNT(CASE WHEN a.Status = 'غائب' THEN 1 END) AS أيام_الغياب,
    COUNT(CASE WHEN a.Status = 'متأخر' THEN 1 END) AS أيام_التأخير,
    COUNT(CASE WHEN a.Status = 'إجازة' THEN 1 END) AS أيام_الإجازة,
    SUM(COALESCE(a.WorkingHours, 0)) AS إجمالي_ساعات_العمل,
    SUM(COALESCE(a.OvertimeHours, 0)) AS إجمالي_الساعات_الإضافية
FROM Employees e
LEFT JOIN Attendance a ON e.Id = a.EmployeeId
LEFT JOIN Departments d ON e.DepartmentId = d.Id
GROUP BY e.Id, strftime('%Y-%m', a.AttendanceDate);

-- عرض أرصدة الإجازات الحالية
CREATE VIEW IF NOT EXISTS vw_CurrentLeaveBalances AS
SELECT 
    e.Id AS EmployeeId,
    e.EmployeeNumber AS رقم_الموظف,
    e.FirstName || ' ' || e.LastName AS اسم_الموظف,
    d.Name AS القسم,
    lt.Name AS نوع_الإجازة,
    lb.Year AS السنة,
    lb.TotalDays AS الرصيد_الكلي,
    lb.UsedDays AS المستخدم,
    lb.RemainingDays AS المتبقي,
    lb.CarriedOverDays AS المرحل
FROM LeaveBalances lb
JOIN Employees e ON lb.EmployeeId = e.Id
JOIN LeaveTypes lt ON lb.LeaveTypeId = lt.Id
LEFT JOIN Departments d ON e.DepartmentId = d.Id
WHERE lb.Year = CAST(strftime('%Y', 'now') AS INTEGER);

-- عرض ملخص الرواتب
CREATE VIEW IF NOT EXISTS vw_PayrollSummary AS
SELECT 
    p.Id AS PayrollId,
    e.EmployeeNumber AS رقم_الموظف,
    e.FirstName || ' ' || e.LastName AS اسم_الموظف,
    d.Name AS القسم,
    p.PayrollYear AS السنة,
    p.PayrollMonth AS الشهر,
    p.BasicSalary AS الراتب_الأساسي,
    p.TotalAllowances AS إجمالي_البدلات,
    p.TotalDeductions AS إجمالي_الخصومات,
    p.OvertimeAmount AS بدل_العمل_الإضافي,
    p.GrossSalary AS إجمالي_الراتب,
    p.NetSalary AS صافي_الراتب,
    p.Status AS الحالة,
    p.PaymentDate AS تاريخ_الدفع
FROM Payroll p
JOIN Employees e ON p.EmployeeId = e.Id
LEFT JOIN Departments d ON e.DepartmentId = d.Id;

-- عرض إحصائيات الأقسام
CREATE VIEW IF NOT EXISTS vw_DepartmentStats AS
SELECT 
    d.Id AS DepartmentId,
    d.Name AS اسم_القسم,
    COUNT(e.Id) AS عدد_الموظفين,
    SUM(CASE WHEN e.EmploymentStatus = 'نشط' THEN 1 ELSE 0 END) AS الموظفين_النشطين,
    AVG(e.BasicSalary) AS متوسط_الراتب,
    SUM(e.BasicSalary) AS إجمالي_الرواتب,
    m.FirstName || ' ' || m.LastName AS مدير_القسم
FROM Departments d
LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsActive = 1
LEFT JOIN Employees m ON d.ManagerId = m.Id
GROUP BY d.Id;

-- عرض الإجازات المعلقة
CREATE VIEW IF NOT EXISTS vw_PendingLeaves AS
SELECT 
    l.Id AS LeaveId,
    e.EmployeeNumber AS رقم_الموظف,
    e.FirstName || ' ' || e.LastName AS اسم_الموظف,
    d.Name AS القسم,
    lt.Name AS نوع_الإجازة,
    l.StartDate AS من_تاريخ,
    l.EndDate AS إلى_تاريخ,
    l.TotalDays AS عدد_الأيام,
    l.Reason AS السبب,
    l.CreatedAt AS تاريخ_الطلب
FROM Leaves l
JOIN Employees e ON l.EmployeeId = e.Id
JOIN LeaveTypes lt ON l.LeaveTypeId = lt.Id
LEFT JOIN Departments d ON e.DepartmentId = d.Id
WHERE l.Status = 'قيد الانتظار'
ORDER BY l.CreatedAt DESC;

-- عرض المستندات المنتهية قريباً
CREATE VIEW IF NOT EXISTS vw_ExpiringDocuments AS
SELECT 
    doc.Id AS DocumentId,
    e.EmployeeNumber AS رقم_الموظف,
    e.FirstName || ' ' || e.LastName AS اسم_الموظف,
    doc.DocumentType AS نوع_المستند,
    doc.DocumentName AS اسم_المستند,
    doc.ExpiryDate AS تاريخ_الانتهاء,
    CAST(julianday(doc.ExpiryDate) - julianday('now') AS INTEGER) AS الأيام_المتبقية
FROM Documents doc
JOIN Employees e ON doc.EmployeeId = e.Id
WHERE doc.ExpiryDate IS NOT NULL 
AND doc.IsActive = 1
AND julianday(doc.ExpiryDate) - julianday('now') <= 30
ORDER BY doc.ExpiryDate;

