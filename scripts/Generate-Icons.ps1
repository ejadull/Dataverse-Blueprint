<#
.SYNOPSIS
    Regenerates icon PNG files from the source blueprint.png.

.DESCRIPTION
    Reads DataverseBlueprint\blueprint.png and produces three resized copies
    in the icons\ directory at the solution root:
      icon-32.png  — SmallImageBase64 (small tool display in XrmToolBox)
      icon-64.png  — NuGet package icon (embedded in .nupkg)
      icon-80.png  — BigImageBase64 (large tool display in XrmToolBox)

.EXAMPLE
    powershell -NoProfile -File scripts\Generate-Icons.ps1
#>

Add-Type -AssemblyName System.Drawing

$solutionRoot = Split-Path $PSScriptRoot -Parent
$srcPath      = Join-Path $solutionRoot "DataverseBlueprint\blueprint.png"
$iconDir      = Join-Path $solutionRoot "icons"

if (-not (Test-Path $srcPath)) {
    Write-Error "Source image not found: $srcPath"
    exit 1
}

function Resize-Image {
    param([System.Drawing.Image]$Source, [int]$Size, [string]$OutputPath)

    $bmp = New-Object System.Drawing.Bitmap($Size, $Size)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode  = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode      = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.PixelOffsetMode    = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.DrawImage($Source, 0, 0, $Size, $Size)
    $g.Dispose()
    $bmp.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "  Created: $OutputPath (${Size}x${Size})"
}

New-Item -ItemType Directory -Force -Path $iconDir | Out-Null
$original = [System.Drawing.Image]::FromFile($srcPath)

Write-Host "Source: $srcPath ($($original.Width)x$($original.Height))"
Write-Host "Output: $iconDir"

Resize-Image -Source $original -Size 32 -OutputPath (Join-Path $iconDir "icon-32.png")
Resize-Image -Source $original -Size 64 -OutputPath (Join-Path $iconDir "icon-64.png")
Resize-Image -Source $original -Size 80 -OutputPath (Join-Path $iconDir "icon-80.png")

$original.Dispose()
Write-Host "Done."
