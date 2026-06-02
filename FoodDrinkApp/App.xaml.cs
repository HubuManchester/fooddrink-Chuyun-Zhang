using FoodDrinkApp.Helpers;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class App : Application
{
    private readonly AccessibilitySettingsService _accessibilitySettings;

    public App(AccessibilitySettingsService accessibilitySettings)
    {
        InitializeComponent();
        _accessibilitySettings = accessibilitySettings;

        // Follow system light/dark theme (course requirement).
        UserAppTheme = AppTheme.Unspecified;

        AccessibilityHelper.ApplySettings(_accessibilitySettings);

        _accessibilitySettings.LargeFontChanged += (_, _) => AccessibilityHelper.ApplySettings(_accessibilitySettings);
        _accessibilitySettings.HighContrastChanged += (_, _) => AccessibilityHelper.ApplySettings(_accessibilitySettings);

        RequestedThemeChanged += (_, _) =>
        {
            AccessibilityHelper.ApplySystemThemeColors();
            AccessibilityHelper.ApplyContrast(_accessibilitySettings.UseHighContrast);
            RefreshVisiblePageThemes();
        };
    }

    private static void RefreshVisiblePageThemes()
    {
        if (Shell.Current?.CurrentPage is ThemedContentPage themedPage)
        {
            themedPage.ApplyTheme();
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Shell must be created here so MauiContext exists before tabs/pages load.
        return new Window(new AppShell());
    }
}
