# Video demo script (5–8 minutes)

Markers do not run your code for marking — the screencast must show each item below.

## 1. Project builds (30 s)

- Open `FoodDrinkApp.sln` in Visual Studio.
- Show **Build → Rebuild Solution** succeeds.
- Run on **Android emulator** or **Windows Machine**.

## 2. Coding standards (45 s)

- In Solution Explorer, show folders: `Models`, `ViewModels`, `Views`, `Services`, `Helpers`.
- Open `.editorconfig` (formatting rules).
- Open `Helpers/NavigationRoutes.cs` (named routes, no magic strings).
- Open `Services/IDataStore.cs` (XML comments on interface).

## 3. Core features (90 s)

**Recipes tab**

- App loads recipe list (SQLite cache).
- Search for a recipe (e.g. `pasta`) — show results.
- Tap **Search** with empty box — show validation alert.
- Toggle **Favorites only**; add/remove a favorite (star).
- Open a recipe — show steps on detail page.

**Novel interactions**

- Tap microphone — **voice search** (say a recipe name).
- Tap **Nearby** — location-based recommendations (allow location when prompted).

## 4. Hardware — five APIs (90 s)

**Hardware tab**

| Button | Hardware | What to say |
|--------|----------|-------------|
| Take food photo | Camera | Show capture or emulator image |
| Get current location | GPS | Show coordinates + status message |
| Recognize speech | Microphone | Speak a word; show result |
| Read sample recipe | Text-to-speech | Hear audio |
| Trigger vibration | Vibration / haptics | Feel or show `HardwareService.Vibrate()` code |

**Detail page (optional)**

- **Read aloud** — TTS on recipe steps.

**Code (15 s)**

- Open `Services/HardwareService.cs` — point out centralized hardware methods.

## 5. Error handling (45 s)

- Empty search → validation dialog (no crash).
- Deny camera or location once → friendly alert from `RunSafeAsync`.
- Open `ViewModels/BaseViewModel.cs` — show `try/catch` and `ShowAlertAsync`.

## 6. Accessibility (30 s)

**Settings tab**

- Toggle large text and high contrast.
- Change app theme (light / dark / system).

## 7. GitHub (30 s)

- Browser: `https://github.com/HubuManchester/fooddrink-Chuyun-Zhang`
- Show **Commits** — multiple messages over time.
- Show `README.md` and `FoodDrinkApp/` structure.

## 8. Close (15 s)

- Restate app name and Food & Drink theme.
- Mention Moodle submission link is the GitHub URL.
