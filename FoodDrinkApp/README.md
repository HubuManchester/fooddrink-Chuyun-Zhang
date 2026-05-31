# FoodDrinkApp

A .NET MAUI mobile app for exploring recipes and nutrition, built for the **Food and Drink** coursework theme using **MVVM**, **XAML UI**, and **on-device hardware APIs**.

## Features

- Recipe list with calories, categories, search, and favorites
- Recipe detail view with step-by-step instructions
- SQLite offline cache for recipes and favorite persistence
- Location-based regional recommendations
- Voice search using the device microphone
- Dedicated hardware demo page for assessment video walkthrough
- Accessibility settings: large text, high contrast, light/dark/system theme

## Mobile hardware used

| Hardware | Where it is used |
|----------|------------------|
| Camera | Capture a food photo on the Hardware Demo page |
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

### Emulator tips for the demo video

- **Camera**: use the emulator extended controls to load a sample image.
- **Location**: set a custom GPS location in emulator settings (e.g. Manchester or Rome).
- **Microphone**: enable host microphone passthrough in the emulator.
- **Vibration**: show the code in `HardwareService.Vibrate()` if the emulator cannot vibrate.

## Accessibility (WCAG)

- Minimum 44pt touch targets on buttons and switches
- Semantic labels and headings for screen readers
- Dynamic theme support (light / dark / system)
- Large text and high contrast toggles in Settings
- User-friendly validation and permission error messages

## Submission requirements (marking checklist)

| Requirement | Status | Evidence in this repo |
|-------------|--------|------------------------|
| Complete .NET MAUI project (builds) | Yes | `FoodDrinkApp.csproj`, `dotnet build -f net9.0-android` or Windows target |
| Naming, formatting, comments | Yes | `.editorconfig`, XML docs on services, `NavigationRoutes.cs` |
| Hardware APIs (4+ recommended) | Yes (5) | `Services/HardwareService.cs` — camera, GPS, microphone, TTS, vibration/haptics |
| Core features + novel interaction | Yes | List, search, detail, favorites, settings; **voice search** and **location-based recommendations** |
| Error handling (try-catch + user message) | Yes | `ViewModels/BaseViewModel.RunSafeAsync`; empty-search validation in `HomeViewModel` |
| GitHub commits + README | Yes | Multiple commits on `main`; this file + root `README.md` |
| Demo video covers all of the above | You record | See [VIDEO_DEMO_SCRIPT.md](VIDEO_DEMO_SCRIPT.md) |

## Assessment checklist (demo video)

- [ ] Demo app on Android phone emulator (or Windows)
- [ ] Show list, detail, search, favorites, settings
- [ ] Show all **5** hardware features (Hardware tab + voice on Home)
- [ ] Show empty-search validation and a permission-denied case
- [ ] Briefly show code: `HardwareService.cs`, `BaseViewModel.cs`, MVVM folders
- [ ] Show GitHub commit history in the browser
- [ ] Submit repository link on Moodle

## Author

Coursework project — Manchester Metropolitan University, Mobile Application Development.
