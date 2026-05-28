using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

[QueryProperty(nameof(RecipeId), nameof(RecipeId))]
public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IDataStore _dataStore;
    private readonly HardwareService _hardwareService;

    [ObservableProperty]
    private FoodItem? _recipe;

    [ObservableProperty]
    private string _recipeId = string.Empty;

    public RecipeDetailViewModel(IDataStore dataStore, HardwareService hardwareService)
    {
        _dataStore = dataStore;
        _hardwareService = hardwareService;
        Title = "Recipe Detail";
    }

    partial void OnRecipeIdChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _ = LoadRecipeAsync();
        }
    }

    [RelayCommand]
    private async Task LoadRecipeAsync()
    {
        await RunSafeAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(RecipeId))
            {
                throw new ArgumentException("Recipe id is missing.");
            }

            Recipe = await _dataStore.GetRecipeAsync(RecipeId);
            if (Recipe is null)
            {
                throw new KeyNotFoundException($"Recipe '{RecipeId}' was not found.");
            }

            Title = Recipe.Name;
            StatusMessage = $"Showing {Recipe.Calories} kcal recipe.";
        }, "Unable to load recipe");
    }

    [RelayCommand]
    private async Task ReadStepsAloudAsync()
    {
        if (Recipe is null)
        {
            return;
        }

        await RunSafeAsync(async () =>
        {
            var text = $"{Recipe.Name}. {Recipe.Description}. Steps. {Recipe.Steps.Replace('\n', ' ')}";
            await _hardwareService.SpeakAsync(text);
            StatusMessage = "Reading recipe steps aloud.";
        }, "Text-to-speech failed");
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Recipe is null)
        {
            return;
        }

        await RunSafeAsync(async () =>
        {
            await _dataStore.ToggleFavoriteAsync(Recipe.Id);
            _hardwareService.Vibrate();
            Recipe = await _dataStore.GetRecipeAsync(Recipe.Id);
            StatusMessage = Recipe?.IsFavorite == true
                ? "Recipe saved to favourites."
                : "Recipe removed from favourites.";
        }, "Unable to update favourite");
    }
}
