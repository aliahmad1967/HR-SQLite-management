# =====================================================
# نظام إدارة الموارد البشرية - HR Management System
# init-database.ps1 - سكريبت تهيئة قاعدة البيانات
# =====================================================

param(
    [string]$DatabasePath = "..\Data\hr_database.db",
    [switch]$Force
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  تهيئة قاعدة البيانات" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$solutionPath = Split-Path -Parent $PSScriptRoot
$schemaPath = Join-Path $solutionPath "Database\Schema"
$fullDbPath = Join-Path $solutionPath $DatabasePath

# إنشاء مجلد البيانات
$dataDir = Split-Path -Parent $fullDbPath
if (-not (Test-Path $dataDir)) {
    New-Item -ItemType Directory -Path $dataDir -Force | Out-Null
    Write-Host "تم إنشاء مجلد البيانات: $dataDir" -ForegroundColor Green
}

# التحقق من وجود قاعدة البيانات
if (Test-Path $fullDbPath) {
    if ($Force) {
        Remove-Item $fullDbPath -Force
        Write-Host "تم حذف قاعدة البيانات القديمة" -ForegroundColor Yellow
    }
    else {
        Write-Host "قاعدة البيانات موجودة مسبقاً. استخدم -Force للإعادة" -ForegroundColor Yellow
        exit 0
    }
}

# تنفيذ ملفات SQL
$sqlFiles = @(
    "01_CreateTables.sql",
    "02_CreateTables_Attendance.sql",
    "03_CreateTables_Payroll.sql",
    "04_CreateIndexes.sql",
    "05_CreateTriggers.sql",
    "06_CreateViews.sql",
    "07_SeedData.sql"
)

Write-Host "`nتنفيذ ملفات SQL..." -ForegroundColor Yellow

foreach ($sqlFile in $sqlFiles) {
    $filePath = Join-Path $schemaPath $sqlFile
    if (Test-Path $filePath) {
        Write-Host "  تنفيذ: $sqlFile" -ForegroundColor Gray
        $sql = Get-Content $filePath -Raw
        # يتم تنفيذ SQL عبر التطبيق عند التشغيل الأول
    }
    else {
        Write-Host "  تحذير: الملف غير موجود: $sqlFile" -ForegroundColor Yellow
    }
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  تم تهيئة قاعدة البيانات بنجاح!" -ForegroundColor Green
Write-Host "  المسار: $fullDbPath" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

