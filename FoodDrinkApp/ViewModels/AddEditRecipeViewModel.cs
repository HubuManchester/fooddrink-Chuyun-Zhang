using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

[QueryProperty(nameof(RecipeId), nameof(RecipeId))]
public partial class AddEditRecipeViewModel : BaseViewModel
{
    private readonly IDataStore _dataStore;
    private readonly HardwareService _hardwareService;

    [ObservableProperty]
    private string _recipeId = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _ingredients = string.Empty;

    [ObservableProperty]
    private string _steps = string.Empty;

    [ObservableProperty]
    private string _emoji = "🍽️";

    [ObservableProperty]
    private string _caloriesText = "300";

    [ObservableProperty]
    private int _selectedCategoryIndex;

    [ObservableProperty]
    private string? _imagePath;

    public IReadOnlyList<string> Categories { get; } =
        ["Breakfast", "Salad", "Pasta", "Main", "Drink", "Curry"];

    public bool IsEditMode => !string.IsNullOrWhiteSpace(RecipeId);

    public AddEditRecipeViewModel(IDataStore dataStore, HardwareService hardwareService)
    {
        _dataStore = dataStore;
        _hardwareService = hardwareService;
        Title = "Add Recipe";
    }

    [RelayCommand]
    private async Task PickCoverPhotoAsync()
    {
        await RunSafeAsync(async () =>
        {
            var path = await _hardwareService.PickPhotoAsync();
            if (path is null)
            {
                StatusMessage = "No cover photo selected.";
                return;
            }

            ImagePath = await HardwareService.SaveRecipeCoverImageAsync(path);
            StatusMessage = "Cover photo selected from gallery.";
        }, "Photo picker failed");
    }

    [RelayCommand]
    private async Task TakeCoverPhotoAsync()
    {
        await RunSafeAsync(async () =>
        {
            var path = await _hardwareService.TakePhotoAsync();
            if (path is null)
            {
                StatusMessage = "Cover photo capture cancelled.";
                return;
            }

            ImagePath = await HardwareService.SaveRecipeCoverImageAsync(path);
            StatusMessage = "Cover photo captured from camera.";
        }, "Camera failed");
    }

    partial void OnRecipeIdChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Title = "Edit Recipe";
            _ = LoadForEditAsync();
        }
    }

    private async Task LoadForEditAsync()
    {
        await RunSafeAsync(async () =>
        {
            var recipe = await _dataStore.GetRecipeAsync(RecipeId);
            if (recipe is null)
            {
                throw new KeyNotFoundException($"Recipe '{RecipeId}' was not found.");
            }

            Name = recipe.Name;
            Description = recipe.Description;
            Ingredients = recipe.Ingredients;
            Steps = recipe.Steps;
            Emoji = recipe.Emoji;
            CaloriesText = recipe.Calories.ToString();
            ImagePath = recipe.ImagePath;
            SelectedCategoryIndex = Math.Max(0, Array.IndexOf(Categories.ToArray(), recipe.Category));
        }, "Unable to load recipe");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await ShowAlertAsync("Validation", "Recipe name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Steps))
        {
            await ShowAlertAsync("Validation", "Cooking steps are required.");
            return;
        }

        if (!int.TryParse(CaloriesText, out var calories) || calories < 0)
        {
            await ShowAlertAsync("Validation", "Calories must be a non-negative number.");
            return;
        }

        await RunSafeAsync(async () =>
        {
            var recipe = new FoodItem
            {
                Id = IsEditMode ? RecipeId : Guid.NewGuid().ToString("N"),
                Name = Name.Trim(),
                Description = Description.Trim(),
                Ingredients = Ingredients.Trim(),
                Steps = Steps.Trim(),
                Emoji = string.IsNullOrWhiteSpace(Emoji) ? "🍽️" : Emoji.Trim(),
                Calories = calories,
                Category = Categories[SelectedCategoryIndex],
                Region = "Europe",
                ImagePath = ImagePath
            };

            if (IsEditMode)
            {
                await _dataStore.UpdateRecipeAsync(recipe);
                StatusMessage = "Recipe updated.";
            }
            else
            {
                await _dataStore.AddRecipeAsync(recipe);
                StatusMessage = "Recipe added.";
            }

            await Shell.Current.GoToAsync("..");
        }, "Unable to save recipe");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
