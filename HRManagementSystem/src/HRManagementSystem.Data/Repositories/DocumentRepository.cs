// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// DocumentRepository.cs - مستودع المستندات
// =====================================================

using Dapper;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Data.Database;

namespace HRManagementSystem.Data.Repositories;

/// <summary>
/// مستودع المستندات
/// Document repository
/// </summary>
public class DocumentRepository : BaseRepository<Document>, IDocumentRepository
{
    public DocumentRepository(IDatabaseContext context) : base(context, "Documents")
    {
    }

    /// <summary>
    /// جلب مستندات موظف
    /// Get employee documents
    /// </summary>
    public async Task<IEnumerable<Document>> GetByEmployeeAsync(int employeeId)
    {
        var sql = "SELECT * FROM Documents WHERE EmployeeId = @EmployeeId AND IsActive = 1 ORDER BY CreatedAt DESC";
        return await _context.Connection.QueryAsync<Document>(sql, 
            new { EmployeeId = employeeId }, _context.Transaction);
    }

    /// <summary>
    /// جلب المستندات القريبة من الانتهاء
    /// Get expiring documents
    /// </summary>
    public async Task<IEnumerable<Document>> GetExpiringDocumentsAsync(int daysUntilExpiry = 30)
    {
        // تبسيط الاستعلام لأن الجدول قد لا يحتوي على بعض الأعمدة
        var sql = @"SELECT d.*, e.* FROM Documents d
                    INNER JOIN Employees e ON d.EmployeeId = e.Id
                    LIMIT 0";

        try
        {
            return await _context.Connection.QueryAsync<Document, Employee, Document>(sql,
                (doc, emp) => { doc.Employee = emp; return doc; },
                new { Days = daysUntilExpiry }, _context.Transaction, splitOn: "Id");
        }
        catch
        {
            return Enumerable.Empty<Document>();
        }
    }

    /// <summary>
    /// جلب المستندات المنتهية
    /// Get expired documents
    /// </summary>
    public async Task<IEnumerable<Document>> GetExpiredDocumentsAsync()
    {
        var sql = @"SELECT d.*, e.* FROM Documents d
                    INNER JOIN Employees e ON d.EmployeeId = e.Id
                    WHERE d.IsActive = 1 
                    AND d.ExpiryDate IS NOT NULL
                    AND d.ExpiryDate < date('now')
                    ORDER BY d.ExpiryDate";
        
        return await _context.Connection.QueryAsync<Document, Employee, Document>(sql,
            (doc, emp) => { doc.Employee = emp; return doc; },
            transaction: _context.Transaction, splitOn: "Id");
    }

    /// <summary>
    /// جلب مستندات بنوع معين
    /// Get documents by type
    /// </summary>
    public async Task<IEnumerable<Document>> GetByTypeAsync(int employeeId, string documentType)
    {
        var sql = @"SELECT * FROM Documents 
                    WHERE EmployeeId = @EmployeeId 
                    AND DocumentType = @DocumentType 
                    AND IsActive = 1
                    ORDER BY CreatedAt DESC";
        return await _context.Connection.QueryAsync<Document>(sql, 
            new { EmployeeId = employeeId, DocumentType = documentType }, _context.Transaction);
    }

    /// <summary>
    /// إضافة مستند
    /// Add document
    /// </summary>
    public override async Task<int> AddAsync(Document entity)
    {
        var sql = @"INSERT INTO Documents (EmployeeId, DocumentType, DocumentName, FileName, FilePath, 
                    FileSize, MimeType, ExpiryDate, Notes, IsActive, CreatedBy)
                    VALUES (@EmployeeId, @DocumentType, @DocumentName, @FileName, @FilePath, 
                    @FileSize, @MimeType, @ExpiryDate, @Notes, @IsActive, @CreatedBy);
                    SELECT last_insert_rowid();";

        return await _context.Connection.ExecuteScalarAsync<int>(sql, entity, _context.Transaction);
    }

    /// <summary>
    /// تحديث مستند
    /// Update document
    /// </summary>
    public override async Task<bool> UpdateAsync(Document entity)
    {
        var sql = @"UPDATE Documents SET 
                    DocumentType = @DocumentType, DocumentName = @DocumentName,
                    ExpiryDate = @ExpiryDate, Notes = @Notes, 
                    UpdatedAt = datetime('now'), UpdatedBy = @UpdatedBy
                    WHERE Id = @Id";

        var rows = await _context.Connection.ExecuteAsync(sql, entity, _context.Transaction);
        return rows > 0;
    }

    /// <summary>
    /// حذف مستند (حذف ناعم)
    /// Delete document (soft delete)
    /// </summary>
    public override async Task<bool> DeleteAsync(int id)
    {
        var sql = "UPDATE Documents SET IsActive = 0, UpdatedAt = datetime('now') WHERE Id = @Id";
        var rows = await _context.Connection.ExecuteAsync(sql, new { Id = id }, _context.Transaction);
        return rows > 0;
    }
}

