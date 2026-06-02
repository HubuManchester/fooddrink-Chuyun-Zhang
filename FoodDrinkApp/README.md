# FoodDrinkApp

A .NET MAUI mobile app for exploring recipes and nutrition, built for the **Food and Drink** coursework theme using **MVVM**, **XAML UI**, and **on-device hardware APIs**.

## Features

- Recipe list with search, category filter, favorites, and pull-to-refresh
- Recipe detail with ingredients, steps, edit, and delete
- **CRUD**: add and edit recipes on `AddEditRecipePage`
- **Dual data load**: `recipes.json` / SQLite first, then Mock API on failure (see `SqliteDataStore.cs`)
- Location-based recommendations and voice search
- **System light/dark theme** follows OS setting (`UserAppTheme = Unspecified`)
- Hardware: camera, GPS (latitude/longitude), microphone, TTS with **Stop** button, vibration
- Accessibility: large text and high contrast

## Mobile hardware used

| Hardware | Where it is used |
|----------|------------------|
| Camera | Pick/take a photo and set it as a **recipe cover** (list + detail); also on Add/Edit recipe |
| Location / Geolocation | Nearby recipe recommendations on Home and Hardware Demo |
| Microphone / Speech recognition | Voice search on Home; speech demo on Hardware Demo |
| Text-to-speech | Read recipe steps on Detail and Hardware Demo pages |
| Vibration / Haptic feedback | Triggered when adding/removing favorites |

All hardware calls are centralized in `Services/HardwareService.cs`.

## Project structure

```
FoodDrinkApp/
├── Models/
├── ViewModels/
├── Views/
├── Services/
├── Helpers/
└── Resources/Styles/
```

## Coding standards

This project follows common C# / .NET conventions:

| Area | Convention used |
|------|-----------------|
| Formatting | `.editorconfig`, 4-space indentation, file-scoped namespaces |
| Naming | PascalCase types/methods; `_camelCase` private fields; `Async` suffix |
| Architecture | MVVM, dependency injection, `IDataStore` abstraction |
| Patterns | `RelayCommand`, centralized `HardwareService`, `RunSafeAsync` error handling |
| Navigation | Route names in `Helpers/NavigationRoutes.cs` |
| Documentation | XML comments on models, services, and interfaces |

## Requirements

- .NET 9 SDK
- .NET MAUI workload (`maui`, `android`, `maui-windows`)
- Visual Studio 2022 17.14+ with Mobile development workload, **or** VS Code + MAUI extension
- Android SDK / emulator for deployment video

## How to run

1. Clone the repository.
2. Open `FoodDrinkApp.sln` or the folder in Visual Studio.
3. Select an Android emulator (phone or tablet) or Windows target.
4. Build and run:

```bash
cd FoodDrinkApp
dotnet build -f net9.0-android
dotnet build -t:Run -f net9.0-android
```

### Recipe covers vs gallery photo

| Source | What you see |
|--------|----------------|
| **Built-in list** | Six coloured cover thumbnails (`Resources/Images/recipes/`) — Salad, Pasta, Teriyaki, etc. |
| **Pick image** | Your **fish and chips** photo in the emulator gallery — run `Scripts/Push-EmulatorPhotos.ps1` after starting the emulator |

Regenerate bundled covers (offline): `.\Scripts\Create-RecipeCovers.ps1`

### Emulator tips for the demo video

- **Camera**: use the emulator extended controls to load a sample image.
- **Text-to-speech (no sound on emulator)**: install **Google Text-to-speech** from Play Store; enable it under Settings → System → Languages → Text-to-speech output; turn up **Media volume** on the emulator side panel; if still silent, cold boot the AVD or set `hw.audioOutput=yes` in the emulator `config.ini`.
- **Location**: set a custom GPS location in emulator settings (e.g. Manchester or Rome).
- **Microphone**: enable host microphone passthrough in the emulator.
- **Vibration**: show the code in `HardwareService.Vibrate()` if the emulator cannot vibrate.

## Accessibility (WCAG)

- Minimum 44pt touch targets on buttons and switches
- Semantic labels and headings for screen readers
- Dynamic theme support (light / dark / system)
- Large text and high contrast toggles in Settings
- User-friendly validation and permission error messages

## Submission (repository & code)

Full pre-submit list (excluding video): see [../SUBMISSION_CHECKLIST.md](../SUBMISSION_CHECKLIST.md) in the repo root.

| Requirement | Status | Evidence |
|-------------|--------|----------|
| .NET MAUI project builds | Yes | `FoodDrinkApp.csproj` |
| Code quality (MVVM, naming) | Yes | `.editorconfig`, `Services/`, `ViewModels/` |
| Hardware (5 APIs) | Yes | `Services/HardwareService.cs` |
| Functionality + novel features | Yes | CRUD, voice search, nearby recommendations |
| Validation & friendly errors | Yes | `BaseViewModel.RunSafeAsync`, `HomeViewModel` search validation |
| Accessibility (WCAG-oriented) | Yes | 44pt targets, Settings, `AccessibilityHelper.cs` |
| Deployment (phone + tablet) | Yes | Android targets; tablet AVD in Device Manager |
| GitHub | Yes | Push to `fooddrink-Chuyun-Zhang` on `main` |

Demo video script (when you record): [VIDEO_DEMO_SCRIPT.md](VIDEO_DEMO_SCRIPT.md)

## Author

Coursework project — Manchester Metropolitan University, Mobile Application Development.
