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

    public SettingsViewModel(AccessibilitySettingsService accessibilitySettings)
    {
        _accessibilitySettings = accessibilitySettings;
        Title = "Accessibility Settings";
        UseLargeFont = _accessibilitySettings.UseLargeFont;
        UseHighContrast = _accessibilitySettings.UseHighContrast;
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

    [RelayCommand]
    private Task ResetAccessibilityAsync()
    {
        UseLargeFont = false;
        UseHighContrast = false;
        StatusMessage = "Accessibility settings reset. Theme follows your system setting.";
        return Task.CompletedTask;
    }
}
