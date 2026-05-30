# FoodDrinkApp Project Development Guide

## 1. Project overview

FoodDrinkApp is a cross-platform .NET MAUI application for the **Food and Drink** coursework theme. It helps users browse recipes, search by name or voice, save favorites, and explore hardware features on mobile devices.

Core capabilities:

- Recipe list with search, refresh, and favorites filter
- Recipe detail page with step-by-step instructions
- SQLite offline cache for recipes and favorite persistence
- Location-based regional recommendations
- Hardware demo page for camera, GPS, speech, TTS, and haptics
- Accessibility settings for large text, high contrast, and theme selection

## 2. Requirement coverage

| Assessment area | Implementation |
|---|---|
| UI/UX and accessibility | XAML pages, tab navigation, semantic labels, theme support, large text and high contrast |
| Mobile hardware | Camera, location, microphone/speech, text-to-speech, vibration/haptics |
| Functionality | List, detail, search, favorites, settings, hardware demo |
| Validation and errors | Empty search validation, permission handling, user-friendly alerts |
| Code quality | MVVM structure with Models, ViewModels, Views, and Services |
| Deployment | Android and Windows targets |
| GitHub usage | Regular commits with clear messages |

## 3. Project structure

```text
FoodDrinkApp/
├── Models/
├── ViewModels/
├── Views/
├── Services/
├── Helpers/          (NavigationRoutes, AccessibilityHelper)
├── Converters/
├── Platforms/
└── Resources/
```

## 4. Key files

- `MauiProgram.cs` — dependency injection and app startup
- `AppShell.xaml` — tab navigation (Recipes, Hardware, Settings)
- `Services/SqliteDataStore.cs` — SQLite cache and favorites
- `Services/HardwareService.cs` — camera, location, speech, TTS, haptics
- `Services/MockDataStore.cs` — seed recipe data
- `ViewModels/` — MVVM logic for each page
- `Views/` — XAML user interface

## 5. Demo video checklist

- Explain the Food and Drink theme and app concept
- Show recipe list, search, favorites, and detail pages
- Demonstrate empty-search validation
- Show all five hardware features on the Hardware Demo page
- Show accessibility settings (large text, high contrast, theme)
- Show Android and/or Windows deployment
- Show GitHub commit history and README
