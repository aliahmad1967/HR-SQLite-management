# =====================================================
# نظام إدارة الموارد البشرية - HR Management System
# build-wpf.ps1 - سكريبت بناء تطبيق WPF
# =====================================================

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "..\dist\wpf"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  بناء نظام إدارة الموارد البشرية (WPF)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# التحقق من وجود .NET SDK
Write-Host "`nالتحقق من .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ: .NET SDK غير مثبت!" -ForegroundColor Red
    exit 1
}
Write-Host "تم العثور على .NET SDK: $dotnetVersion" -ForegroundColor Green

# الانتقال إلى مجلد الحل
$solutionPath = Split-Path -Parent $PSScriptRoot
Set-Location $solutionPath

# استعادة الحزم
Write-Host "`nاستعادة حزم NuGet..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ في استعادة الحزم!" -ForegroundColor Red
    exit 1
}
Write-Host "تم استعادة الحزم بنجاح" -ForegroundColor Green

# بناء المشروع
Write-Host "`nبناء المشروع..." -ForegroundColor Yellow
dotnet build -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ في البناء!" -ForegroundColor Red
    exit 1
}
Write-Host "تم البناء بنجاح" -ForegroundColor Green

# نشر التطبيق
Write-Host "`nنشر التطبيق..." -ForegroundColor Yellow
dotnet publish src/HRManagementSystem/HRManagementSystem.csproj `
    -c $Configuration `
    -o $OutputPath `
    --self-contained true `
    -r win-x64 `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ في النشر!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  تم البناء والنشر بنجاح!" -ForegroundColor Green
Write-Host "  المخرجات في: $OutputPath" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

