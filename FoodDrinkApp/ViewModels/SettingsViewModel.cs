using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Helpers;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly AccessibilitySettingsService _accessibilitySettings;

    [ObservableProperty]
    private bool _useLargeFont;

    [ObservableProperty]
    private bool _useHighContrast;

    [ObservableProperty]
    private int _selectedThemeIndex;

    public IReadOnlyList<string> ThemeOptions { get; } =
        ["Follow system", "Light", "Dark"];

    public SettingsViewModel(AccessibilitySettingsService accessibilitySettings)
    {
        _accessibilitySettings = accessibilitySettings;
        Title = "Accessibility Settings";
        UseLargeFont = _accessibilitySettings.UseLargeFont;
        UseHighContrast = _accessibilitySettings.UseHighContrast;
        SelectedThemeIndex = _accessibilitySettings.SelectedTheme switch
        {
            AppTheme.Light => 1,
            AppTheme.Dark => 2,
            _ => 0
        };
    }

    partial void OnUseLargeFontChanged(bool value)
    {
        _accessibilitySettings.UseLargeFont = value;
        AccessibilityHelper.ApplyFontScale(value);
        StatusMessage = value ? "Large text enabled." : "Standard text restored.";
    }

    partial void OnUseHighContrastChanged(bool value)
    {
        _accessibilitySettings.UseHighContrast = value;
        AccessibilityHelper.ApplyContrast(value);
        StatusMessage = value ? "High contrast enabled." : "Standard contrast restored.";
    }

    partial void OnSelectedThemeIndexChanged(int value)
    {
        _accessibilitySettings.SelectedTheme = value switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
        AccessibilityHelper.ApplyContrast(UseHighContrast);
        StatusMessage = $"Theme set to {ThemeOptions[value]}.";
    }

    [RelayCommand]
    private Task ResetAccessibilityAsync()
    {
        UseLargeFont = false;
        UseHighContrast = false;
        SelectedThemeIndex = 0;
        StatusMessage = "Accessibility settings reset.";
        return Task.CompletedTask;
    }
}
