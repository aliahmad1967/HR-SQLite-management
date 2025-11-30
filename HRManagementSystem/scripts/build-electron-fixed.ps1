# HR Management System - Electron Build Script

param(
    [string]$Configuration = "Release"
)

Write-Host "========================================"  -ForegroundColor Cyan
Write-Host "  Building HR Management System (Electron)" -ForegroundColor Cyan
Write-Host "========================================"  -ForegroundColor Cyan

# Check .NET SDK
Write-Host "`nChecking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: .NET SDK not installed!" -ForegroundColor Red
    exit 1
}
Write-Host "Found .NET SDK: $dotnetVersion" -ForegroundColor Green

# Check Node.js
Write-Host "`nChecking Node.js..." -ForegroundColor Yellow
$nodeVersion = node --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Node.js not installed!" -ForegroundColor Red
    exit 1
}
Write-Host "Found Node.js: $nodeVersion" -ForegroundColor Green

# Check ElectronNET.CLI
Write-Host "`nChecking ElectronNET.CLI..." -ForegroundColor Yellow
$electronCli = dotnet tool list -g | Select-String "electronnet.cli"
if (-not $electronCli) {
    Write-Host "Installing ElectronNET.CLI..." -ForegroundColor Yellow
    dotnet tool install ElectronNET.CLI -g
}
Write-Host "ElectronNET.CLI ready" -ForegroundColor Green

# Navigate to project folder
$solutionPath = Split-Path -Parent $PSScriptRoot
$webProjectPath = Join-Path $solutionPath "src\HRManagementSystem.Web"
Set-Location $webProjectPath

# Restore packages
Write-Host "`nRestoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error restoring packages!" -ForegroundColor Red
    exit 1
}
Write-Host "Packages restored successfully" -ForegroundColor Green

# Initialize Electron
Write-Host "`nInitializing Electron..." -ForegroundColor Yellow
electronize init
if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Init may already exist" -ForegroundColor Yellow
}

# Build Electron app
Write-Host "`nBuilding Electron app for Windows..." -ForegroundColor Yellow
electronize build /target win /package-json electron.manifest.json

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error building Electron!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================"  -ForegroundColor Green
Write-Host "  Build completed successfully!" -ForegroundColor Green
Write-Host "  Output in: bin/Desktop/" -ForegroundColor Green
Write-Host "========================================"  -ForegroundColor Green

