using FoodDrinkApp.Services;

namespace FoodDrinkApp.Helpers;

/// <summary>
/// Applies shared theme resource colors whenever a page appears (system light/dark + accessibility).
/// </summary>
public class ThemedContentPage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        if (Application.Current?.Resources is null)
        {
            return;
        }

        AccessibilityHelper.ApplySystemThemeColors();

        if (Handler?.MauiContext?.Services.GetService<AccessibilitySettingsService>() is { } settings)
        {
            AccessibilityHelper.ApplyContrast(settings.UseHighContrast);
        }

        BackgroundColor = (Color)Application.Current.Resources["PageBackgroundColor"];
    }
}
