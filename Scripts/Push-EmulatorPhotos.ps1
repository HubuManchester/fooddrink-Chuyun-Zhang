# Pushes sample food photos into the Android emulator gallery for Pick image testing.
# Usage: start the emulator, then run: .\Scripts\Push-EmulatorPhotos.ps1

$adb = "${env:LOCALAPPDATA}\Android\Sdk\platform-tools\adb.exe"
if (-not (Test-Path $adb)) {
    $adb = "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe"
}
if (-not (Test-Path $adb)) {
    Write-Error "adb.exe not found. Install Android SDK platform-tools."
    exit 1
}

$repoRoot = Split-Path $PSScriptRoot -Parent
$fish = Join-Path $repoRoot "FoodDrinkApp\Resources\Images\recipes\teriyaki.png"
if (-not (Test-Path $fish)) {
    Write-Error "Sample image not found: $fish"
    exit 1
}

& $adb shell "mkdir -p /sdcard/Pictures/Demo"
& $adb push $fish "/sdcard/Pictures/Demo/food_sample.png"
& $adb shell "am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///sdcard/Pictures/Demo/food_sample.png"
Write-Host "Done. Close and reopen Pick image in the app."
