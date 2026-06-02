# FoodDrinkApp Project Development Guide

## 1. Project overview

FoodDrinkApp is a cross-platform .NET MAUI application for the **Food and Drink** coursework theme. It helps users browse recipes, search by name or voice, save favorites, and explore hardware features on mobile devices.

Core capabilities:

- Recipe list with search, refresh, and favorites filter
- Recipe detail with step-by-step instructions and read-aloud
- Full **CRUD** (add, edit, delete recipes)
- SQLite offline cache with `recipes.json` and Mock API fallback
- Location-based regional recommendations
- Recipe cover photos (gallery, camera, or bundled images)
- Hardware demo page for camera, GPS, speech, TTS, and haptics
- Accessibility: large text, high contrast, system light/dark theme

## 2. Assessment alignment (Dr Davison criteria)

| Assessment area | Weight | Implementation |
|-----------------|--------|----------------|
| UI/UX & accessibility | 30% | XAML UI, WCAG-oriented 44pt targets, semantic labels, theme + contrast settings |
| Mobile hardware | 20% | Five APIs in `HardwareService.cs` (camera, GPS, mic, TTS, vibration/haptics) |
| Functionality | 20% | List, detail, CRUD, favorites, voice search, nearby recommendations |
| Validation & errors | 10% | `BaseViewModel.RunSafeAsync`, form validation, permission-friendly alerts |
| Code quality | 10% | MVVM, DI, `.editorconfig`, shared services, XML documentation |
| Deployment | 5% | Android phone + **Android tablet** emulator (or second device) |
| GitHub usage | 5% | This repo with README and iterative commits |

See [SUBMISSION_CHECKLIST.md](SUBMISSION_CHECKLIST.md) for a pre-submit list (excluding video).

## 3. Project structure

```text
FoodDrinkApp/
├── Models/
├── ViewModels/
├── Views/
├── Services/          HardwareService, SqliteDataStore, Mock API
├── Helpers/           NavigationRoutes, AccessibilityHelper, ThemedContentPage
├── Converters/
├── Platforms/         Android TTS, Windows TTS, manifests
└── Resources/
    ├── Images/recipes/   Bundled cover photos
    ├── Raw/recipes.json  Seed data
    └── Styles/
```

## 4. Key files

| File | Purpose |
|------|---------|
| `MauiProgram.cs` | DI registration |
| `App.xaml.cs` | Theme + `CreateWindow` / AppShell startup |
| `AppShell.xaml` | Tab navigation |
| `Services/HardwareService.cs` | All hardware APIs |
| `Services/SqliteDataStore.cs` | SQLite + local/API load |
| `ViewModels/BaseViewModel.cs` | Error handling pattern |
| `Helpers/AccessibilityHelper.cs` | WCAG-related resource updates |

## 5. Deployment

### Phone (primary)

1. Open `FoodDrinkApp.sln`.
2. Select a phone AVD (e.g. Pixel 7, API 34).
3. F5 — grant camera, location, and microphone when prompted.

### Tablet (deployment mark)

1. Android Device Manager → **New** or **Start** a tablet AVD (e.g. Pixel Tablet).
2. Select the tablet as run target → F5.
3. Confirm all three tabs load (Recipes, Hardware, Settings).

### Windows (optional)

Use **Windows Machine** for reliable text-to-speech during demos.

## 6. Emulator tips

- **Gallery**: Do not drag files into the system photo picker. Drag to the emulator window, or run `Scripts/Push-EmulatorPhotos.ps1`.
- **GPS**: Emulator extended controls → Location → custom coordinates (e.g. Manchester).
- **TTS**: Enable Google Text-to-speech and media volume; use Windows if still silent.
- **Vibration**: If the emulator cannot vibrate, markers accept showing `HardwareService.Vibrate()` in code.

## 7. GitHub workflow

Work in this repository only. Commit after each meaningful feature (English messages, your own author name). Push to `origin/main` before Moodle submission.
