# HR Management System - Build Installer Script

Write-Host "========================================"  -ForegroundColor Cyan
Write-Host "  Building HR Management System Installer" -ForegroundColor Cyan
Write-Host "========================================"  -ForegroundColor Cyan

$solutionPath = Split-Path -Parent $PSScriptRoot

# Step 1: Build and publish the application
Write-Host "`nStep 1: Publishing application..." -ForegroundColor Yellow
Set-Location $solutionPath

dotnet publish src/HRManagementSystem/HRManagementSystem.csproj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error publishing application!" -ForegroundColor Red
    exit 1
}
Write-Host "Application published successfully!" -ForegroundColor Green

# Step 2: Create dist folder
Write-Host "`nStep 2: Creating dist folder..." -ForegroundColor Yellow
$distPath = Join-Path $solutionPath "dist"
if (-not (Test-Path $distPath)) {
    New-Item -ItemType Directory -Path $distPath | Out-Null
}

# Step 3: Check for Inno Setup
Write-Host "`nStep 3: Checking Inno Setup..." -ForegroundColor Yellow
$innoSetupPath = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
$innoSetupPath2 = "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"

if (Test-Path $innoSetupPath) {
    $iscc = $innoSetupPath
} elseif (Test-Path $innoSetupPath2) {
    $iscc = $innoSetupPath2
} else {
    Write-Host "Inno Setup not found. Creating portable ZIP instead..." -ForegroundColor Yellow
    
    # Create ZIP file
    $zipPath = Join-Path $distPath "HRManagementSystem_1.0.0_Portable.zip"
    Compress-Archive -Path "$solutionPath\publish\*" -DestinationPath $zipPath -Force
    
    Write-Host "`n========================================"  -ForegroundColor Green
    Write-Host "  Portable ZIP created successfully!" -ForegroundColor Green
    Write-Host "  Output: $zipPath" -ForegroundColor Green
    Write-Host "========================================"  -ForegroundColor Green
    exit 0
}

# Step 4: Build installer with Inno Setup
Write-Host "`nStep 4: Building installer with Inno Setup..." -ForegroundColor Yellow
$issPath = Join-Path $solutionPath "installer\setup.iss"

& $iscc $issPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error building installer!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================"  -ForegroundColor Green
Write-Host "  Installer created successfully!" -ForegroundColor Green
Write-Host "  Output in: dist/" -ForegroundColor Green
Write-Host "========================================"  -ForegroundColor Green

