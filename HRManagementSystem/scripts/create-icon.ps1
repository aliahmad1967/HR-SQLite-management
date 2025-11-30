# Create Application Icon

Add-Type -AssemblyName System.Drawing

# Create a 256x256 bitmap
$bitmap = New-Object System.Drawing.Bitmap(256, 256)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)

# Set high quality
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

# Background - Blue gradient simulation
$bgBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0, 102, 204))
$graphics.FillEllipse($bgBrush, 10, 10, 236, 236)

# Inner circle - lighter blue
$innerBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(51, 153, 255))
$graphics.FillEllipse($innerBrush, 30, 30, 196, 196)

# HR Text
$font = New-Object System.Drawing.Font("Arial", 80, [System.Drawing.FontStyle]::Bold)
$textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
$stringFormat = New-Object System.Drawing.StringFormat
$stringFormat.Alignment = [System.Drawing.StringAlignment]::Center
$stringFormat.LineAlignment = [System.Drawing.StringAlignment]::Center
$rect = New-Object System.Drawing.RectangleF(0, 0, 256, 256)
$graphics.DrawString("HR", $font, $textBrush, $rect, $stringFormat)

# Save as PNG first
$pngPath = Join-Path $PSScriptRoot "..\src\HRManagementSystem\Resources\app.png"
$bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)

# Convert to ICO
$icoPath = Join-Path $PSScriptRoot "..\src\HRManagementSystem\Resources\app.ico"

# Create icon from bitmap
$icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())

# Save icon using file stream
$fs = [System.IO.FileStream]::new($icoPath, [System.IO.FileMode]::Create)
$icon.Save($fs)
$fs.Close()

# Cleanup
$graphics.Dispose()
$bitmap.Dispose()
$bgBrush.Dispose()
$innerBrush.Dispose()
$textBrush.Dispose()
$font.Dispose()

Write-Host "Icon created successfully!" -ForegroundColor Green
Write-Host "Icon path: $icoPath" -ForegroundColor Green

