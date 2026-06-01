using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Helpers;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class HardwareDemoViewModel : BaseViewModel
{
    private readonly HardwareService _hardwareService;
    private readonly IDataStore _dataStore;

    [ObservableProperty]
    private string? _photoPath;

    [ObservableProperty]
    private string _locationText = "Location not requested yet.";

    [ObservableProperty]
    private string _speechResult = "Speech recognition has not run yet.";

    [ObservableProperty]
    private bool _isSpeaking;

    [ObservableProperty]
    private string _sampleRecipeSteps =
        "Step one, prepare fresh ingredients. Step two, cook gently. Step three, serve immediately.";

    public HardwareDemoViewModel(HardwareService hardwareService, IDataStore dataStore)
    {
        _hardwareService = hardwareService;
        _dataStore = dataStore;
        Title = "Hardware Demo";
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        await RunSafeAsync(async () =>
        {
            PhotoPath = await _hardwareService.TakePhotoAsync();
            StatusMessage = PhotoPath is null
                ? "Camera capture cancelled."
                : "Photo captured. Tap \"Use as recipe cover photo\" below to save it to a recipe.";
        }, "Camera failed");
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        await RunSafeAsync(async () =>
        {
            PhotoPath = await _hardwareService.PickPhotoAsync();
            StatusMessage = PhotoPath is null
                ? "No photo selected from library."
                : "Photo selected. Tap \"Use as recipe cover photo\" below to save it to a recipe.";
        }, "Photo picker failed");
    }

    [RelayCommand]
    private async Task AttachPhotoToRecipeAsync()
    {
        if (string.IsNullOrWhiteSpace(PhotoPath))
        {
            await ShowAlertAsync("Recipe cover photo", "Pick or take a photo first.");
            return;
        }

        await RunSafeAsync(async () =>
        {
            var recipes = (await _dataStore.GetRecipesAsync()).ToList();
            if (recipes.Count == 0)
            {
                await ShowAlertAsync("Recipe cover photo", "No recipes found. Add a recipe on the Recipes tab first.");
                return;
            }

            var names = recipes.Select(static r => r.Name).ToArray();
            var chosenName = await Shell.Current.DisplayActionSheet(
                "Set this photo as the cover for which recipe?",
                "Cancel",
                null,
                names);

            if (string.IsNullOrWhiteSpace(chosenName) || chosenName == "Cancel")
            {
                StatusMessage = "Cover photo not assigned.";
                return;
            }

            var recipe = recipes.FirstOrDefault(r => r.Name == chosenName);
            if (recipe is null)
            {
                return;
            }

            recipe.ImagePath = await HardwareService.SaveRecipeCoverImageAsync(PhotoPath);
            await _dataStore.UpdateRecipeAsync(recipe);
            StatusMessage = $"Cover photo saved for {recipe.Name}. Open Recipes to see it.";

            var open = await Shell.Current.DisplayAlert(
                "Cover photo saved",
                $"The photo is now the cover for \"{recipe.Name}\". View recipe detail?",
                "View recipe",
                "Stay here");

            if (open)
            {
                await Shell.Current.GoToAsync(
                    $"{NavigationRoutes.RecipeDetail}?{NavigationRoutes.RecipeIdQueryKey}={recipe.Id}");
            }
        }, "Unable to save cover photo");
    }

    [RelayCommand]
    private async Task GetLocationAsync()
    {
        await RunSafeAsync(async () =>
        {
            var location = await _hardwareService.GetCurrentLocationAsync();
            if (location is null)
            {
                LocationText = "Location could not be determined.";
                return;
            }

            LocationText = $"GPS: Latitude {location.Latitude:F6}, Longitude {location.Longitude:F6}";
            var recommendations = await _dataStore.GetNearbyRecommendationsAsync(location.Latitude, location.Longitude);
            StatusMessage = recommendations.Count == 0
                ? "Location acquired, but no regional recipes were found."
                : $"Location acquired. Nearby suggestion: {recommendations[0].Name}.";
        }, "Location failed");
    }

    [RelayCommand]
    private async Task ReadRecipeAsync()
    {
        if (IsBusy)
        {
            await ShowAlertAsync("Please wait", "Another operation is still running. Try again in a moment.");
            return;
        }

        try
        {
            IsBusy = true;
            IsSpeaking = true;
            StatusMessage = "Reading aloud... (uses speaker, not microphone)";
            await _hardwareService.SpeakAsync(SampleRecipeSteps);
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
    private void StopReading()
    {
        _hardwareService.StopSpeaking();
        IsSpeaking = false;
        StatusMessage = "Reading stopped.";
    }

    [RelayCommand]
    private async Task RecognizeSpeechAsync()
    {
        await RunSafeAsync(async () =>
        {
            StatusMessage = "Listening for a recipe name...";
            var result = await _hardwareService.RecognizeSpeechAsync();
            SpeechResult = string.IsNullOrWhiteSpace(result)
                ? "No speech detected."
                : result;
            StatusMessage = "Speech recognition completed.";
        }, "Speech recognition failed");
    }

    [RelayCommand]
    private Task VibrateAsync()
    {
        return RunSafeAsync(() =>
        {
            _hardwareService.Vibrate();
            StatusMessage = "Haptic feedback triggered.";
            return Task.CompletedTask;
        }, "Haptic feedback failed");
    }
}
