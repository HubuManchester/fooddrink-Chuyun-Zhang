# Renames recipe cover files to MAUI-safe lowercase names (required for Android build).
$dir = Join-Path $PSScriptRoot "..\Resources\Images\recipes"
if (-not (Test-Path $dir)) {
    Write-Error "Folder not found: $dir"
    exit 1
}

$required = @(
    "mediterranean_salad.png",
    "carbonara.png",
    "teriyaki.png",
    "berry_smoothie.png",
    "vegetable_curry.png",
    "avocado_toast.png"
)

foreach ($file in Get-ChildItem $dir -File) {
    $lower = $file.Name.ToLower()
    if ($file.Name -ceq $lower) { continue }

    $temp = Join-Path $dir ("_rename_" + $lower)
    $final = Join-Path $dir $lower
    Copy-Item $file.FullName $temp -Force
    Remove-Item $file.FullName -Force
    if (Test-Path $final) { Remove-Item $final -Force }
    Move-Item $temp $final -Force
    Write-Host "Renamed: $($file.Name) -> $lower"
}

Write-Host ""
Write-Host "Required file names:"
$required | ForEach-Object { Write-Host "  $_" }
Write-Host ""
Get-ChildItem $dir -File | Select-Object -ExpandProperty Name
