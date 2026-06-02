# Submission checklist (non-video)

Use this list before submitting the **GitHub repository link** on Moodle. Video recording is separate.

## Repository

- [ ] Latest code is on `main` at https://github.com/HubuManchester/fooddrink-Chuyun-Zhang
- [ ] `FoodDrinkApp.sln` opens and **Rebuild Solution** succeeds
- [ ] Commits show steady development (not a single upload)
- [ ] Author is **Chuyun Zhang** (no third-party co-author lines)

## Marking criteria (code & repo only)

| Criterion | Weight | Evidence in repo |
|-----------|--------|------------------|
| UI/UX & accessibility | 30% | `Resources/Styles/Styles.xaml` (44pt targets), `AccessibilityHelper.cs`, Settings page, `SemanticProperties` on pages |
| Mobile hardware | 20% | `Services/HardwareService.cs` — camera, GPS, microphone, TTS, vibration/haptics (5 features) |
| Functionality | 20% | Home, detail, CRUD, voice search, nearby recommendations, Hardware tab |
| Validation & errors | 10% | `BaseViewModel.RunSafeAsync`, empty-search alert, permission messages, Add/Edit validation |
| Code quality | 10% | MVVM folders, `.editorconfig`, DI, `IDataStore`, XML comments |
| Deployment | 5% | Android phone + tablet targets in `.csproj`; see tablet steps below |
| GitHub usage | 5% | This repository with README and commit history |

## Deployment (tablet — required for 5% mark)

1. Open **Android Device Manager** in Visual Studio.
2. Create or start a **tablet** AVD (e.g. Pixel Tablet, API 34).
3. Run `FoodDrinkApp` on the tablet and confirm Recipes / Hardware / Settings tabs load.

Phone emulator steps are the same with a phone AVD.

## Run targets

| Target | Use for |
|--------|---------|
| Android phone emulator | Primary demo, camera, gallery, GPS spoof |
| Android tablet emulator | Deployment mark (brief run) |
| Windows Machine | TTS if Android emulator audio is silent |

## Hardware notes for markers

| Feature | Emulator |
|---------|----------|
| Camera / gallery | Extended controls or photos in `/sdcard/Pictures/Demo/` |
| GPS | Set custom location in emulator |
| Microphone | Enable host mic passthrough |
| TTS | Often silent on Android emulator — use Windows or show `HardwareService.cs` |
| Vibration | Show `HardwareService.Vibrate()` in code if device does not vibrate |

## Learning outcomes

- **LO1**: .NET MAUI app runs on Android and Windows (`FoodDrinkApp.csproj`).
- **LO2**: On-device hardware via `HardwareService` and platform TTS helpers.

## Moodle

Submit the **repository URL** only (unless your module leader specifies otherwise).
