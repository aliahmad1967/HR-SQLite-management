# =====================================================
# نظام إدارة الموارد البشرية - HR Management System
# run-dev.ps1 - سكريبت تشغيل بيئة التطوير
# =====================================================

param(
    [ValidateSet("wpf", "electron")]
    [string]$Mode = "wpf"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  تشغيل نظام إدارة الموارد البشرية" -ForegroundColor Cyan
Write-Host "  الوضع: $Mode" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$solutionPath = Split-Path -Parent $PSScriptRoot
Set-Location $solutionPath

if ($Mode -eq "wpf") {
    Write-Host "`nتشغيل تطبيق WPF..." -ForegroundColor Yellow
    dotnet run --project src/HRManagementSystem/HRManagementSystem.csproj
}
elseif ($Mode -eq "electron") {
    Write-Host "`nتشغيل تطبيق Electron..." -ForegroundColor Yellow
    Set-Location "src/HRManagementSystem.Web"
    electronize start
}

