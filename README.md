[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/uM_GSLJS)

# FoodDrinkApp

A .NET MAUI recipe application for the **Food and Drink** coursework theme (Manchester Metropolitan University). The app uses **MVVM**, **XAML**, SQLite caching, and five on-device hardware APIs.

## Features

- Recipe list with search, category filter, favorites, and pull-to-refresh
- Recipe detail with ingredients, steps, read-aloud, edit, and delete
- Full CRUD on `AddEditRecipePage`
- Recipe cover photos (bundled images, camera capture, or gallery pick)
- Voice search and location-based nearby recommendations
- System light/dark theme, large text, and high-contrast accessibility settings
- Hardware demo tab for camera, GPS, speech, TTS, and haptics

## Mobile hardware (5 APIs)

All hardware access is centralized in `FoodDrinkApp/Services/HardwareService.cs`.

| Hardware | Usage |
|----------|--------|
| Camera | Take photo (manual shutter) or pick from gallery; set recipe cover |
| Location | Nearby recommendations on Home and Hardware tab |
| Microphone | Voice search (Home) and speech recognition demo |
| Text-to-speech | Read recipe steps on Detail and Hardware tab (with Stop) |
| Vibration / haptics | Favorites toggle and Hardware demo |

## Project structure

```
FoodDrinkApp/
├── Models/
├── ViewModels/
├── Views/
├── Services/       IDataStore, SqliteDataStore, HardwareService, Mock API
├── Helpers/        NavigationRoutes, AccessibilityHelper, ThemedContentPage
├── Converters/
├── Platforms/      Android TTS, Windows TTS, manifests
└── Resources/      recipes.json, recipe cover images, styles
```

## Requirements

- .NET 9 SDK and .NET MAUI workload (`maui`, `android`, `maui-windows`)
- Visual Studio 2022 17.14+ with Mobile development workload
- Android SDK / emulator (phone and tablet for deployment demo)

## How to run

1. Open `FoodDrinkApp.sln` in Visual Studio 2022.
2. Select **Android Emulator** (phone or tablet) or **Windows Machine**.
3. Press **F5** to build and run.

```bash
dotnet build FoodDrinkApp/FoodDrinkApp.csproj -f net9.0-android
```

### Recipe cover images

Bundled covers live in `FoodDrinkApp/Resources/Images/recipes/` and must use **lowercase** file names:

`mediterranean_salad.png`, `carbonara.png`, `teriyaki.png`, `berry_smoothie.png`, `vegetable_curry.png`, `avocado_toast.png`

### Data loading

`SqliteDataStore` loads in order: SQLite cache → embedded `recipes.json` → Mock API → built-in seed data. Pull-to-refresh tries the Mock API first. Optional endpoint:

```csharp
MockApiOptions.RecipesEndpoint = "https://YOUR_PROJECT.mockapi.io/api/v1/recipes";
```

Enable **Demo: simulate local JSON failure** on the Recipes tab to show API fallback in a screencast.

### Emulator notes

- **Camera**: extended controls → Camera, or pick an image from the gallery.
- **GPS**: set a custom location in emulator settings.
- **TTS**: use Windows target if the Android emulator has no audio; install Google Text-to-speech and raise media volume on Android.
- **Vibration**: show `HardwareService.Vibrate()` in code if the emulator cannot vibrate.

## Accessibility (WCAG-oriented)

- Minimum 44pt touch targets (`Resources/Styles/Styles.xaml`)
- Semantic labels on Hardware and Home pages
- System theme, large text, and high contrast (Settings tab)
- User-friendly validation and permission messages via `BaseViewModel.RunSafeAsync`

## Assessment alignment

| Criterion | Evidence |
|-----------|----------|
| UI/UX & accessibility | Theme settings, 44pt targets, semantic labels |
| Mobile hardware (5) | `HardwareService.cs` |
| Functionality | CRUD, search, favorites, voice search, nearby recipes |
| Validation & errors | Empty search alert, form validation, permission handling |
| Code quality | MVVM, DI, `.editorconfig`, XML comments |
| Deployment | Android phone + tablet emulators |
| GitHub | Commit history on `main` |

## Author

Chuyun Zhang — Mobile Application Development coursework.
