# Mock API configuration

## Dual-load strategy (local first, API fallback)

`SqliteDataStore.LoadRecipesAsync()` implements:

1. **SQLite cache** (if data exists)
2. **recipes.json** embedded resource
3. **catch** → **Mock API** (`MockApiRecipeClient`)
4. **catch** → built-in seed data

Pull-to-refresh tries the Mock API first, then falls back to local JSON.

## Configure mockapi.io

1. Create a `recipes` resource at [mockapi.io](https://mockapi.io).
2. Set the endpoint in code before the app loads data:

```csharp
// MauiProgram.cs or App startup
MockApiOptions.RecipesEndpoint = "https://YOUR_PROJECT.mockapi.io/api/v1/recipes";
```

Suggested JSON fields per item: `name`, `description`, `emoji`, `calories`, `category`, `region`, `ingredients`, `steps`.

## Demo local failure switch

On the **Recipes** tab, enable **Demo: simulate local JSON failure** to force the Mock API path for your screencast (shows try/catch fallback).
