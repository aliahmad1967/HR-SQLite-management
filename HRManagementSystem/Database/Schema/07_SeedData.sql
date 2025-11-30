-- =====================================================
-- البيانات الأولية - Seed Data
-- =====================================================

-- إدراج أنواع الإجازات
INSERT OR IGNORE INTO LeaveTypes (Id, Name, NameEn, DefaultDays, IsPaid, RequiresApproval, Color) VALUES
(1, 'إجازة سنوية', 'Annual Leave', 21, 1, 1, '#4CAF50'),
(2, 'إجازة مرضية', 'Sick Leave', 30, 1, 1, '#F44336'),
(3, 'إجازة بدون راتب', 'Unpaid Leave', 0, 0, 1, '#9E9E9E'),
(4, 'إجازة طوارئ', 'Emergency Leave', 5, 1, 1, '#FF9800'),
(5, 'إجازة زواج', 'Marriage Leave', 5, 1, 1, '#E91E63'),
(6, 'إجازة أمومة', 'Maternity Leave', 70, 1, 1, '#9C27B0'),
(7, 'إجازة أبوة', 'Paternity Leave', 3, 1, 1, '#3F51B5'),
(8, 'إجازة وفاة', 'Bereavement Leave', 5, 1, 0, '#607D8B'),
(9, 'إجازة حج', 'Hajj Leave', 15, 1, 1, '#00BCD4');

-- إدراج عناصر الراتب الأساسية
INSERT OR IGNORE INTO SalaryComponents (Id, Name, NameEn, Type, CalculationType, DefaultAmount, DefaultPercentage, IsTaxable, SortOrder) VALUES
(1, 'بدل سكن', 'Housing Allowance', 'بدل', 'نسبة من الراتب', 0, 25, 0, 1),
(2, 'بدل مواصلات', 'Transportation Allowance', 'بدل', 'مبلغ ثابت', 500, 0, 0, 2),
(3, 'بدل طعام', 'Food Allowance', 'بدل', 'مبلغ ثابت', 300, 0, 0, 3),
(4, 'بدل هاتف', 'Phone Allowance', 'بدل', 'مبلغ ثابت', 200, 0, 0, 4),
(5, 'تأمينات اجتماعية', 'Social Insurance', 'خصم', 'نسبة من الراتب', 0, 9.75, 0, 1),
(6, 'ضريبة الدخل', 'Income Tax', 'خصم', 'نسبة من الراتب', 0, 0, 1, 2),
(7, 'سلف', 'Advance Payment', 'خصم', 'مبلغ ثابت', 0, 0, 0, 3),
(8, 'غياب', 'Absence Deduction', 'خصم', 'مبلغ ثابت', 0, 0, 0, 4);

-- إدراج إعدادات النظام
INSERT OR IGNORE INTO SystemSettings (SettingKey, SettingValue, SettingType, Description) VALUES
('company_name', 'شركة الموارد البشرية', 'string', 'اسم الشركة'),
('company_name_en', 'HR Company', 'string', 'Company Name'),
('working_hours_per_day', '8', 'number', 'ساعات العمل اليومية'),
('overtime_rate', '1.5', 'number', 'معدل الساعات الإضافية'),
('currency', 'ريال', 'string', 'العملة'),
('date_format', 'yyyy-MM-dd', 'string', 'تنسيق التاريخ'),
('fiscal_year_start', '01-01', 'string', 'بداية السنة المالية'),
('leave_carry_over_max', '5', 'number', 'الحد الأقصى لترحيل الإجازات'),
('employee_id_prefix', 'EMP', 'string', 'بادئة رقم الموظف'),
('employee_id_digits', '6', 'number', 'عدد أرقام رقم الموظف');

-- إدراج قسم افتراضي
INSERT OR IGNORE INTO Departments (Id, Name, NameEn, Description, IsActive) VALUES
(1, 'الإدارة العامة', 'General Management', 'القسم الرئيسي للشركة', 1),
(2, 'الموارد البشرية', 'Human Resources', 'قسم إدارة شؤون الموظفين', 1),
(3, 'المالية', 'Finance', 'قسم الشؤون المالية والمحاسبة', 1),
(4, 'تقنية المعلومات', 'Information Technology', 'قسم تقنية المعلومات', 1),
(5, 'المبيعات', 'Sales', 'قسم المبيعات والتسويق', 1);

-- إدراج مسميات وظيفية افتراضية
INSERT OR IGNORE INTO Positions (Id, Title, TitleEn, DepartmentId, MinSalary, MaxSalary) VALUES
(1, 'مدير عام', 'General Manager', 1, 20000, 50000),
(2, 'مدير موارد بشرية', 'HR Manager', 2, 12000, 25000),
(3, 'مسؤول توظيف', 'Recruitment Officer', 2, 6000, 12000),
(4, 'مدير مالي', 'Finance Manager', 3, 15000, 30000),
(5, 'محاسب', 'Accountant', 3, 5000, 12000),
(6, 'مدير تقنية المعلومات', 'IT Manager', 4, 15000, 30000),
(7, 'مطور برمجيات', 'Software Developer', 4, 8000, 20000),
(8, 'مدير مبيعات', 'Sales Manager', 5, 12000, 25000),
(9, 'مندوب مبيعات', 'Sales Representative', 5, 4000, 10000);

-- إدراج مستخدم افتراضي (كلمة المرور: Admin@123)
-- Password Hash generated using BCrypt
INSERT OR IGNORE INTO Users (Id, Username, PasswordHash, Email, Role, IsActive) VALUES
(1, 'admin', '$2a$11$K5b7p5X5V5X5V5X5V5X5VeK5b7p5X5V5X5V5X5V5X5VeK5b7p5X5', 'admin@hrms.com', 'مدير النظام', 1);

-- إدراج أنواع المستندات كإعدادات
INSERT OR IGNORE INTO SystemSettings (SettingKey, SettingValue, SettingType, Description) VALUES
('document_types', 'هوية وطنية,جواز سفر,رخصة قيادة,شهادة جامعية,شهادة خبرة,عقد عمل,شهادة صحية,إقامة,تأمين طبي,أخرى', 'list', 'أنواع المستندات');

