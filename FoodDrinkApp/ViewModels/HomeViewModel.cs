using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Helpers;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IDataStore _dataStore;
    private readonly HardwareService _hardwareService;
    private List<FoodItem> _allRecipes = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showFavoritesOnly;

    [ObservableProperty]
    private int _selectedCategoryIndex;

    [ObservableProperty]
    private bool _simulateLocalFailure;

    public ObservableCollection<FoodItem> Recipes { get; } = [];

    public IReadOnlyList<string> Categories { get; }

    public HomeViewModel(IDataStore dataStore, HardwareService hardwareService)
    {
        _dataStore = dataStore;
        _hardwareService = hardwareService;
        Categories = _dataStore.GetCategories();
        Title = "Recipe Explorer";
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        await RunSafeAsync(async () =>
        {
            MockApiOptions.SimulateLocalFailureForDemo = SimulateLocalFailure;
            var result = await _dataStore.LoadRecipesAsync();
            _allRecipes = result.Recipes.ToList();
            ApplyFilters();
            StatusMessage = $"Loaded {Recipes.Count} recipes from {result.DataSource}.";
        }, "Unable to load recipes");
    }

    [RelayCommand]
    private async Task RefreshRecipesAsync()
    {
        await RunSafeAsync(async () =>
        {
            var result = await _dataStore.LoadRecipesAsync(forceRefresh: true);
            _allRecipes = result.Recipes.ToList();
            ApplyFilters();
            StatusMessage = $"Refreshed {Recipes.Count} recipes from {result.DataSource}.";
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
                $"GPS: {location.Latitude:F4}, {location.Longitude:F4} — {recommendations.Count} regional suggestions.";
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

        await Shell.Current.GoToAsync($"{NavigationRoutes.RecipeDetail}?{NavigationRoutes.RecipeIdQueryKey}={item.Id}");
    }

    [RelayCommand]
    private async Task AddRecipeAsync()
    {
        await Shell.Current.GoToAsync(NavigationRoutes.AddEditRecipe);
    }

    partial void OnShowFavoritesOnlyChanged(bool value) => ApplyFilters();

    partial void OnSelectedCategoryIndexChanged(int value) => ApplyFilters();

    partial void OnSimulateLocalFailureChanged(bool value)
    {
        MockApiOptions.SimulateLocalFailureForDemo = value;
        StatusMessage = value
            ? "Demo mode: next load will skip local JSON and try Mock API."
            : "Demo mode off: local JSON loads first.";
    }

    private void ApplyFilters()
    {
        IEnumerable<FoodItem> filtered = _allRecipes;

        if (SelectedCategoryIndex > 0 && SelectedCategoryIndex < Categories.Count)
        {
            var category = Categories[SelectedCategoryIndex];
            filtered = filtered.Where(r => string.Equals(r.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        if (ShowFavoritesOnly)
        {
            filtered = filtered.Where(r => r.IsFavorite);
        }

        ReplaceRecipes(filtered);
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
