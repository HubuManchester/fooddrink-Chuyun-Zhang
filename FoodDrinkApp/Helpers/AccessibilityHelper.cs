using FoodDrinkApp.Services;

namespace FoodDrinkApp.Helpers;

/// <summary>
/// Applies WCAG-friendly accessibility settings to shared application resources.
/// </summary>
public static class AccessibilityHelper
{
    private const double LargeFontScale = 1.35;

    public static void ApplySettings(AccessibilitySettingsService settings)
    {
        if (Application.Current?.Resources is null)
        {
            return;
        }

        ApplyFontScale(settings.UseLargeFont);
        ApplySystemThemeColors();
        ApplyContrast(settings.UseHighContrast);
    }

    /// <summary>Updates resource colors when the OS switches light/dark mode.</summary>
    public static void ApplySystemThemeColors()
    {
        if (Application.Current?.Resources is null)
        {
            return;
        }

        var isDark = Application.Current.RequestedTheme == AppTheme.Dark;
        Application.Current.Resources["PageBackgroundColor"] = isDark ? Color.FromArgb("#1F1F1F") : Colors.White;
        Application.Current.Resources["PrimaryTextColor"] = isDark ? Colors.White : Color.FromArgb("#1A1A1A");
        Application.Current.Resources["SecondaryTextColor"] = isDark ? Color.FromArgb("#D0D0D0") : Color.FromArgb("#555555");
        Application.Current.Resources["CardBackgroundColor"] = isDark ? Color.FromArgb("#2A2A2A") : Color.FromArgb("#F7F7F7");
        Application.Current.Resources["AccentColor"] = isDark ? Color.FromArgb("#81C784") : Color.FromArgb("#2E7D32");
        Application.Current.Resources["PrimaryButtonColor"] = isDark ? Color.FromArgb("#388E3C") : Color.FromArgb("#2E7D32");
        Application.Current.Resources["PrimaryButtonTextColor"] = Colors.White;
    }

    public static void ApplyFontScale(bool useLargeFont)
    {
        if (Application.Current?.Resources is null)
        {
            return;
        }

        Application.Current.Resources["BaseFontSize"] = useLargeFont ? 18.0 : 14.0;
        Application.Current.Resources["TitleFontSize"] = useLargeFont ? 28.0 : 22.0;
        Application.Current.Resources["SubtitleFontSize"] = useLargeFont ? 20.0 : 16.0;
    }

    public static void ApplyContrast(bool useHighContrast)
    {
        if (Application.Current?.Resources is null)
        {
            return;
        }

        if (useHighContrast)
        {
            Application.Current.Resources["PageBackgroundColor"] = Colors.Black;
            Application.Current.Resources["PrimaryTextColor"] = Colors.White;
            Application.Current.Resources["SecondaryTextColor"] = Colors.White;
            Application.Current.Resources["CardBackgroundColor"] = Color.FromArgb("#111111");
            Application.Current.Resources["AccentColor"] = Colors.Yellow;
            Application.Current.Resources["PrimaryButtonColor"] = Colors.Yellow;
            Application.Current.Resources["PrimaryButtonTextColor"] = Colors.Black;
            return;
        }

        ApplySystemThemeColors();
    }

    public static double GetFontScale(bool useLargeFont) => useLargeFont ? LargeFontScale : 1.0;
}
