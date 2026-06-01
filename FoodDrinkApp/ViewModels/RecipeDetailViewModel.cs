using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Helpers;
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

    [ObservableProperty]
    private bool _isSpeaking;

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

        if (IsBusy)
        {
            await ShowAlertAsync("Please wait", "Another operation is still running. Try again in a moment.");
            return;
        }

        try
        {
            IsBusy = true;
            IsSpeaking = true;
            StatusMessage = "Reading aloud...";
            var text = $"{Recipe.Name}. Ingredients: {Recipe.Ingredients}. Steps: {Recipe.Steps.Replace('\n', ' ')}";
            await _hardwareService.SpeakAsync(text);
            StatusMessage = "Finished reading aloud.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Text-to-speech failed: {GetFriendlyMessage(ex)}";
            await ShowAlertAsync("Something went wrong", StatusMessage);
        }
        finally
        {
            IsSpeaking = false;
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void StopReadingAloud()
    {
        _hardwareService.StopSpeaking();
        IsSpeaking = false;
        StatusMessage = "Reading stopped.";
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
                ? "Recipe saved to favorites."
                : "Recipe removed from favorites.";
        }, "Unable to update favorite");
    }

    [RelayCommand]
    private async Task EditRecipeAsync()
    {
        if (Recipe is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{NavigationRoutes.AddEditRecipe}?{NavigationRoutes.RecipeIdQueryKey}={Recipe.Id}");
    }

    [RelayCommand]
    private async Task DeleteRecipeAsync()
    {
        if (Recipe is null || Shell.Current is null)
        {
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Delete recipe",
            $"Delete \"{Recipe.Name}\" permanently?",
            "Delete",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        await RunSafeAsync(async () =>
        {
            await _dataStore.DeleteRecipeAsync(Recipe.Id);
            StatusMessage = "Recipe deleted.";
            await Shell.Current.GoToAsync("..");
        }, "Unable to delete recipe");
    }
}
