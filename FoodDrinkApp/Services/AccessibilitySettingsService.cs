namespace FoodDrinkApp.Services;

/// <summary>
/// Persists accessibility preferences such as large text and high contrast.
/// </summary>
public class AccessibilitySettingsService
{
    private const string LargeFontKey = "accessibility_large_font";
    private const string HighContrastKey = "accessibility_high_contrast";
    private const string ThemeKey = "app_theme";

    public bool UseLargeFont
    {
        get => Preferences.Default.Get(LargeFontKey, false);
        set
        {
            Preferences.Default.Set(LargeFontKey, value);
            LargeFontChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool UseHighContrast
    {
        get => Preferences.Default.Get(HighContrastKey, false);
        set
        {
            Preferences.Default.Set(HighContrastKey, value);
            HighContrastChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public AppTheme SelectedTheme
    {
        get => Preferences.Default.Get(ThemeKey, (int)AppTheme.Unspecified) switch
        {
            (int)AppTheme.Light => AppTheme.Light,
            (int)AppTheme.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
        set
        {
            Preferences.Default.Set(ThemeKey, (int)value);
            if (Application.Current is not null)
            {
                Application.Current.UserAppTheme = value;
            }

            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? LargeFontChanged;
    public event EventHandler? HighContrastChanged;
    public event EventHandler? ThemeChanged;
}
