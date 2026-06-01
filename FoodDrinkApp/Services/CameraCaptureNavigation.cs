using FoodDrinkApp.Helpers;

namespace FoodDrinkApp.Services;

/// <summary>
/// Opens the in-app camera page and returns the saved photo path when the user captures or cancels.
/// </summary>
public static class CameraCaptureNavigation
{
    private static TaskCompletionSource<string?>? _completion;

    public static async Task<string?> CaptureAsync()
    {
        if (Shell.Current is null)
        {
            throw new InvalidOperationException("Shell is not ready for camera navigation.");
        }

        _completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        await Shell.Current.GoToAsync(NavigationRoutes.CameraCapture);
        return await _completion.Task;
    }

    public static void Complete(string? path) => _completion?.TrySetResult(path);
}
