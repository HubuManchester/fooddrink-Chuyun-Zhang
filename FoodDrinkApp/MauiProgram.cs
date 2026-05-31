using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Media;
using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;
using FoodDrinkApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace FoodDrinkApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ISpeechToText>(SpeechToText.Default);
        builder.Services.AddSingleton<AccessibilitySettingsService>();
        builder.Services.AddSingleton<HardwareService>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<MockApiRecipeClient>();
        builder.Services.AddSingleton<IDataStore, SqliteDataStore>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddTransient<AddEditRecipeViewModel>();
        builder.Services.AddTransient<HardwareDemoViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<RecipeDetailPage>();
        builder.Services.AddTransient<AddEditRecipePage>();
        builder.Services.AddTransient<HardwareDemoPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
