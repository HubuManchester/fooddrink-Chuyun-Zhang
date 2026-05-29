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

## Assessment checklist

- [ ] Demo app on Android phone emulator
- [ ] Demo same app on Android tablet emulator (or Windows)
- [ ] Show list, detail, search, favorites, settings
- [ ] Show all 5 hardware features
- [ ] Show validation (empty search) and permission denial handling
- [ ] Push regular commits to GitHub
- [ ] Submit repository link on Moodle

## Author

Coursework project — Manchester Metropolitan University, Mobile Application Development.
