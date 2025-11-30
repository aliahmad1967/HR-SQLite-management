// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Document.cs - نموذج المستند
// =====================================================

namespace HRManagementSystem.Core.Models;

/// <summary>
/// نموذج المستند
/// Document model
/// </summary>
public class Document : ActivatableEntity
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// نوع المستند
    /// Document type
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// اسم المستند
    /// Document name
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// اسم الملف
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// مسار الملف
    /// File path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// حجم الملف (بايت)
    /// File size in bytes
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// نوع الملف (MIME)
    /// MIME type
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// تاريخ الانتهاء
    /// Expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    // ==================== Navigation Properties ====================

    public Employee? Employee { get; set; }

    // ==================== Calculated Properties ====================

    /// <summary>
    /// هل منتهي الصلاحية؟
    /// Is expired?
    /// </summary>
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate < DateTime.Now;

    /// <summary>
    /// هل قريب الانتهاء؟ (خلال 30 يوم)
    /// Is expiring soon? (within 30 days)
    /// </summary>
    public bool IsExpiringSoon => ExpiryDate.HasValue && 
        ExpiryDate >= DateTime.Now && 
        ExpiryDate <= DateTime.Now.AddDays(30);

    /// <summary>
    /// حجم الملف منسق
    /// Formatted file size
    /// </summary>
    public string FileSizeFormatted
    {
        get
        {
            if (!FileSize.HasValue) return "غير محدد";
            var size = FileSize.Value;
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int i = 0;
            decimal dSize = size;
            while (dSize >= 1024 && i < suffixes.Length - 1)
            {
                dSize /= 1024;
                i++;
            }
            return $"{dSize:N2} {suffixes[i]}";
        }
    }

    /// <summary>
    /// الأيام المتبقية للانتهاء
    /// Days until expiry
    /// </summary>
    public int? DaysUntilExpiry => ExpiryDate.HasValue ? 
        (int)(ExpiryDate.Value - DateTime.Now).TotalDays : null;

    /// <summary>
    /// هل صورة؟
    /// Is image?
    /// </summary>
    public bool IsImage => MimeType?.StartsWith("image/") == true;

    /// <summary>
    /// هل PDF؟
    /// Is PDF?
    /// </summary>
    public bool IsPdf => MimeType == "application/pdf";
}

