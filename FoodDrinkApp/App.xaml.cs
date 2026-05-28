using FoodDrinkApp.Helpers;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class App : Application
{
    private readonly AppShell _shell;
    private readonly AccessibilitySettingsService _accessibilitySettings;

    public App(AppShell shell, AccessibilitySettingsService accessibilitySettings)
    {
        InitializeComponent();
        _shell = shell;
        _accessibilitySettings = accessibilitySettings;

        AccessibilityHelper.ApplySettings(_accessibilitySettings);

        _accessibilitySettings.LargeFontChanged += (_, _) => AccessibilityHelper.ApplySettings(_accessibilitySettings);
        _accessibilitySettings.HighContrastChanged += (_, _) => AccessibilityHelper.ApplySettings(_accessibilitySettings);
        _accessibilitySettings.ThemeChanged += (_, _) => AccessibilityHelper.ApplySettings(_accessibilitySettings);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_shell);
    }
}
