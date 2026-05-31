using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
                ? "Photo capture was cancelled."
                : "Food photo captured successfully.";
        }, "Camera failed");
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
        await RunSafeAsync(async () =>
        {
            await _hardwareService.SpeakAsync(SampleRecipeSteps);
            StatusMessage = "Recipe steps are being read aloud.";
        }, "Text-to-speech failed");
    }

    [RelayCommand]
    private void StopReading()
    {
        _hardwareService.StopSpeaking();
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
