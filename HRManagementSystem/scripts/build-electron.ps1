# =====================================================
# نظام إدارة الموارد البشرية - HR Management System
# build-electron.ps1 - سكريبت بناء تطبيق Electron
# =====================================================

param(
    [string]$Configuration = "Release"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  بناء نظام إدارة الموارد البشرية (Electron)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# التحقق من وجود .NET SDK
Write-Host "`nالتحقق من .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ: .NET SDK غير مثبت!" -ForegroundColor Red
    exit 1
}
Write-Host "تم العثور على .NET SDK: $dotnetVersion" -ForegroundColor Green

# التحقق من وجود Node.js
Write-Host "`nالتحقق من Node.js..." -ForegroundColor Yellow
$nodeVersion = node --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ: Node.js غير مثبت!" -ForegroundColor Red
    exit 1
}
Write-Host "تم العثور على Node.js: $nodeVersion" -ForegroundColor Green

# التحقق من وجود ElectronNET.CLI
Write-Host "`nالتحقق من ElectronNET.CLI..." -ForegroundColor Yellow
$electronCli = dotnet tool list -g | Select-String "electronnet.cli"
if (-not $electronCli) {
    Write-Host "تثبيت ElectronNET.CLI..." -ForegroundColor Yellow
    dotnet tool install ElectronNET.CLI -g
}
Write-Host "ElectronNET.CLI جاهز" -ForegroundColor Green

# الانتقال إلى مجلد المشروع
$solutionPath = Split-Path -Parent $PSScriptRoot
$webProjectPath = Join-Path $solutionPath "src\HRManagementSystem.Web"
Set-Location $webProjectPath

# استعادة الحزم
Write-Host "`nاستعادة حزم NuGet..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ في استعادة الحزم!" -ForegroundColor Red
    exit 1
}
Write-Host "تم استعادة الحزم بنجاح" -ForegroundColor Green

# تهيئة Electron
Write-Host "`nتهيئة Electron..." -ForegroundColor Yellow
electronize init
if ($LASTEXITCODE -ne 0) {
    Write-Host "تحذير: قد تكون التهيئة موجودة مسبقاً" -ForegroundColor Yellow
}

# بناء تطبيق Electron
Write-Host "`nبناء تطبيق Electron لـ Windows..." -ForegroundColor Yellow
electronize build /target win /package-json electron.manifest.json

if ($LASTEXITCODE -ne 0) {
    Write-Host "خطأ في بناء Electron!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  تم البناء بنجاح!" -ForegroundColor Green
Write-Host "  المخرجات في: dist/" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

