// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// PositionRepository.cs - مستودع المسميات الوظيفية
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع المسميات الوظيفية
/// Position repository
/// </summary>
public class PositionRepository : BaseRepository<Position>, IPositionRepository
{
    public PositionRepository(IDatabaseContext context) : base(context, "Positions")
    {
    }

    /// <summary>
    /// جلب المسميات الوظيفية لقسم معين
    /// Get positions by department
    /// </summary>
    public async Task<IEnumerable<Position>> GetByDepartmentAsync(int departmentId)
    {
        var sql = "SELECT * FROM Positions WHERE DepartmentId = @DepartmentId AND IsActive = 1 ORDER BY Name";
        return await _context.Connection.QueryAsync<Position>(sql, 
            new { DepartmentId = departmentId }, _context.Transaction);
    }

    /// <summary>
    /// جلب المسميات الوظيفية النشطة
    /// Get active positions
    /// </summary>
    public async Task<IEnumerable<Position>> GetActivePositionsAsync()
    {
        var sql = @"SELECT p.*, d.* FROM Positions p
                    LEFT JOIN Departments d ON p.DepartmentId = d.Id
                    WHERE p.IsActive = 1 
                    ORDER BY d.Name, p.Name";
        
        return await _context.Connection.QueryAsync<Position, Department, Position>(sql,
            (position, department) => { position.Department = department; return position; },
            transaction: _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// إضافة مسمى وظيفي
    /// Add position
    /// </summary>
    public override async Task<int> AddAsync(Position entity)
    {
        var sql = @"INSERT INTO Positions (Name, NameEn, Description, DepartmentId, MinSalary, MaxSalary, IsActive)
                    VALUES (@Name, @NameEn, @Description, @DepartmentId, @MinSalary, @MaxSalary, @IsActive);
                    SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }

    /// <summary>
    /// تحديث مسمى وظيفي
    /// Update position
    /// </summary>
    public override async Task<bool> UpdateAsync(Position entity)
    {
        var sql = @"UPDATE Positions SET 
                    Name = @Name, NameEn = @NameEn, Description = @Description,
                    DepartmentId = @DepartmentId, MinSalary = @MinSalary, MaxSalary = @MaxSalary,
                    IsActive = @IsActive, UpdatedAt = datetime('now')
                    WHERE Id = @Id";

        var rows = await _context.Connection.ExecuteAsync(sql, entity, _context.Transaction);
        return rows > 0;
    }
}

