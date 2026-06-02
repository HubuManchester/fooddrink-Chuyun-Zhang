# Generates bundled recipe cover thumbnails (offline, no network).
# Fish-and-chips is NOT used here — that photo is for emulator gallery only (Push-EmulatorPhotos.ps1).

Add-Type -AssemblyName System.Drawing

$dir = Join-Path $PSScriptRoot "..\Resources\Images\recipes"
New-Item -ItemType Directory -Force -Path $dir | Out-Null

$covers = @(
    @{ File = "mediterranean_salad.png"; Color = [Drawing.Color]::FromArgb(76, 175, 80);  Label = "Salad" },
    @{ File = "carbonara.png";          Color = [Drawing.Color]::FromArgb(255, 152, 0); Label = "Pasta" },
    @{ File = "teriyaki.png";           Color = [Drawing.Color]::FromArgb(121, 85, 72);  Label = "Teriyaki" },
    @{ File = "berry_smoothie.png";     Color = [Drawing.Color]::FromArgb(233, 30, 99);  Label = "Smoothie" },
    @{ File = "vegetable_curry.png";    Color = [Drawing.Color]::FromArgb(255, 193, 7);  Label = "Curry" },
    @{ File = "avocado_toast.png";      Color = [Drawing.Color]::FromArgb(139, 195, 74); Label = "Avocado" }
)

foreach ($c in $covers) {
    $bmp = New-Object System.Drawing.Bitmap 480, 480
    $graphics = [System.Drawing.Graphics]::FromImage($bmp)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.Clear($c.Color)

    $font = New-Object System.Drawing.Font("Segoe UI", 42, [System.Drawing.FontStyle]::Bold)
    $brush = [System.Drawing.Brushes]::White
    $size = $graphics.MeasureString($c.Label, $font)
    $x = (480 - $size.Width) / 2
    $y = (480 - $size.Height) / 2
    $graphics.DrawString($c.Label, $font, $brush, $x, $y)

    $path = Join-Path $dir $c.File
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $bmp.Dispose()
    Write-Host "Created $path"
}

Write-Host "Done. Rebuild the app or pull-to-refresh if you already ran it once."
