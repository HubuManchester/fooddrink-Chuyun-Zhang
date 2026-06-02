# Pushes your fish-and-chips photo into the Android emulator gallery for Pick image only.
# Bundled recipe covers are separate files under Resources/Images/recipes/.
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
$fish = Join-Path $repoRoot "FoodDrinkApp\Resources\Images\emulator_samples\fish_and_chips.png"

# Fallback: copy from Cursor assets if sample folder is empty
if (-not (Test-Path $fish)) {
    $assets = Join-Path $env:USERPROFILE ".cursor\projects\c-Users-Administrator-FoodDrinkApp\assets"
    $assetFile = Get-ChildItem $assets -Filter '*7cdc2ccef38e33c5ce86a93c69e9061*' -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($assetFile) {
        $destDir = Split-Path $fish -Parent
        New-Item -ItemType Directory -Force -Path $destDir | Out-Null
        $bytes = [System.IO.File]::ReadAllBytes('\\?\' + $assetFile.FullName)
        [System.IO.File]::WriteAllBytes($fish, $bytes)
    }
}

if (-not (Test-Path $fish)) {
    Write-Error "fish_and_chips.png not found. Place your photo at: $fish"
    exit 1
}

& $adb shell "mkdir -p /sdcard/Pictures/Demo"
& $adb push $fish "/sdcard/Pictures/Demo/fish_and_chips.png"
& $adb shell "am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///sdcard/Pictures/Demo/fish_and_chips.png"
Write-Host "Done. Close and reopen Pick image in the app."
