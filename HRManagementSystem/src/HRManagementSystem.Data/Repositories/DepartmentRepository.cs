// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DepartmentRepository.cs - مستودع الأقسام
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع الأقسام
/// Department repository
/// </summary>
public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(IDatabaseContext context) : base(context, "Departments")
    {
    }

    /// <summary>
    /// جلب قسم بالاسم
    /// Get department by name
    /// </summary>
    public async Task<Department?> GetByNameAsync(string name)
    {
        var sql = "SELECT * FROM Departments WHERE Name = @Name";
        return await _context.Connection.QueryFirstOrDefaultAsync<Department>(sql, 
            new { Name = name }, _context.Transaction);
    }

    /// <summary>
    /// جلب الأقسام النشطة
    /// Get active departments
    /// </summary>
    public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
    {
        var sql = "SELECT * FROM Departments WHERE IsActive = 1 ORDER BY Name";
        return await _context.Connection.QueryAsync<Department>(sql, transaction: _context.Transaction);
    }

    /// <summary>
    /// جلب قسم مع موظفيه
    /// Get department with employees
    /// </summary>
    public async Task<Department?> GetWithEmployeesAsync(int id)
    {
        var sql = @"
            SELECT d.*, e.*
            FROM Departments d
            LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsActive = 1
            WHERE d.Id = @Id";

        var departmentDict = new Dictionary<int, Department>();

        await _context.Connection.QueryAsync<Department, Employee, Department>(
            sql,
            (department, employee) =>
            {
                if (!departmentDict.TryGetValue(department.Id, out var currentDept))
                {
                    currentDept = department;
                    currentDept.Employees = new List<Employee>();
                    departmentDict.Add(currentDept.Id, currentDept);
                }

                if (employee != null)
                {
                    ((List<Employee>)currentDept.Employees).Add(employee);
                }

                return currentDept;
            },
            new { Id = id },
            _context.Transaction,
            splitOn: "Id"
        );

        return departmentDict.Values.FirstOrDefault();
    }

    /// <summary>
    /// جلب الهيكل الهرمي للأقسام
    /// Get department hierarchy
    /// </summary>
    public async Task<IEnumerable<Department>> GetHierarchyAsync()
    {
        var sql = @"
            WITH RECURSIVE dept_hierarchy AS (
                SELECT *, 0 as Level
                FROM Departments
                WHERE ParentDepartmentId IS NULL
                UNION ALL
                SELECT d.*, dh.Level + 1
                FROM Departments d
                INNER JOIN dept_hierarchy dh ON d.ParentDepartmentId = dh.Id
            )
            SELECT * FROM dept_hierarchy ORDER BY Level, Name";

        return await _context.Connection.QueryAsync<Department>(sql, transaction: _context.Transaction);
    }

    /// <summary>
    /// إضافة قسم جديد
    /// Add new department
    /// </summary>
    public override async Task<int> AddAsync(Department entity)
    {
        var sql = @"INSERT INTO Departments (Name, Description, ManagerId, IsActive)
                    VALUES (@Name, @Description, @ManagerId, @IsActive);
                    SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }

    /// <summary>
    /// تحديث قسم
    /// Update department
    /// </summary>
    public override async Task<bool> UpdateAsync(Department entity)
    {
        var sql = @"UPDATE Departments SET
                    Name = @Name, Description = @Description,
                    ManagerId = @ManagerId,
                    IsActive = @IsActive, UpdatedAt = datetime('now')
                    WHERE Id = @Id";

        var rows = await _context.Connection.ExecuteAsync(sql, entity, _context.Transaction);
        return rows > 0;
    }
}

