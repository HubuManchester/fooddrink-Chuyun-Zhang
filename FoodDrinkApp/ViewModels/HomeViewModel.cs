using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IDataStore _dataStore;
    private readonly HardwareService _hardwareService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showFavoritesOnly;

    public ObservableCollection<FoodItem> Recipes { get; } = [];

    public HomeViewModel(IDataStore dataStore, HardwareService hardwareService)
    {
        _dataStore = dataStore;
        _hardwareService = hardwareService;
        Title = "Recipe Explorer";
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        await RunSafeAsync(async () =>
        {
            var recipes = await _dataStore.GetRecipesAsync();
            ReplaceRecipes(recipes);
            StatusMessage = $"Loaded {Recipes.Count} recipes from offline cache.";
        }, "Unable to load recipes");
    }

    [RelayCommand]
    private async Task RefreshRecipesAsync()
    {
        await RunSafeAsync(async () =>
        {
            var recipes = await _dataStore.GetRecipesAsync(forceRefresh: true);
            ReplaceRecipes(recipes);
            StatusMessage = "Recipe list refreshed successfully.";
        }, "Unable to refresh recipes");
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await ShowAlertAsync("Validation", "Please enter a recipe name before searching.");
            return;
        }

        await RunSafeAsync(async () =>
        {
            var results = await _dataStore.SearchRecipesAsync(SearchText);
            ReplaceRecipes(results);
            StatusMessage = results.Count == 0
                ? "No recipes matched your search."
                : $"Found {results.Count} matching recipes.";
        }, "Search failed");
    }

    [RelayCommand]
    private async Task VoiceSearchAsync()
    {
        await RunSafeAsync(async () =>
        {
            StatusMessage = "Listening... speak a recipe name.";
            var spokenText = await _hardwareService.RecognizeSpeechAsync();
            if (string.IsNullOrWhiteSpace(spokenText))
            {
                await ShowAlertAsync("Voice search", "No speech was detected. Please try again.");
                return;
            }

            SearchText = spokenText;
            var results = await _dataStore.SearchRecipesAsync(spokenText);
            ReplaceRecipes(results);
            StatusMessage = $"Voice search heard \"{spokenText}\" and found {results.Count} results.";
        }, "Voice search failed");
    }

    [RelayCommand]
    private async Task NearbyRecommendationsAsync()
    {
        await RunSafeAsync(async () =>
        {
            var location = await _hardwareService.GetCurrentLocationAsync();
            if (location is null)
            {
                await ShowAlertAsync("Location unavailable", "Could not determine your current location.");
                return;
            }

            var recommendations = await _dataStore.GetNearbyRecommendationsAsync(location.Latitude, location.Longitude);
            ReplaceRecipes(recommendations);
            StatusMessage =
                $"Showing {recommendations.Count} nearby suggestions for {location.Latitude:F2}, {location.Longitude:F2}.";
        }, "Location recommendations failed");
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(FoodItem? item)
    {
        if (item is null)
        {
            return;
        }

        await RunSafeAsync(async () =>
        {
            var wasFavorite = item.IsFavorite;
            await _dataStore.ToggleFavoriteAsync(item.Id);
            _hardwareService.Vibrate();
            await LoadRecipesAsync();
            StatusMessage = wasFavorite
                ? $"Removed {item.Name} from favorites."
                : $"Added {item.Name} to favorites.";
        }, "Unable to update favorite");
    }

    [RelayCommand]
    private async Task OpenRecipeAsync(FoodItem? item)
    {
        if (item is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(Views.RecipeDetailPage)}?RecipeId={item.Id}");
    }

    partial void OnShowFavoritesOnlyChanged(bool value)
    {
        _ = ApplyFavoriteFilterAsync();
    }

    private async Task ApplyFavoriteFilterAsync()
    {
        await RunSafeAsync(async () =>
        {
            var recipes = await _dataStore.GetRecipesAsync();
            ReplaceRecipes(ShowFavoritesOnly ? recipes.Where(r => r.IsFavorite) : recipes);
        }, "Unable to filter favorites");
    }

    private void ReplaceRecipes(IEnumerable<FoodItem> recipes)
    {
        Recipes.Clear();
        foreach (var recipe in recipes)
        {
            Recipes.Add(recipe);
        }
    }
}
