using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using FoodDrinkApp.Helpers;
using FoodDrinkApp.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;

namespace FoodDrinkApp.Views;

public partial class CameraCapturePage : ThemedContentPage
{
    private bool _finished;
    private bool _previewStarted;
    private TaskCompletionSource<Stream?>? _captureCompletion;

    public CameraCapturePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_previewStarted)
        {
            return;
        }

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Camera permission", "Camera access is required to take a photo.", "OK");
            await CloseAsync(null);
            return;
        }

        try
        {
            await PreviewCamera.StartCameraPreview(CancellationToken.None);
            _previewStarted = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Camera unavailable", ex.Message, "OK");
            await CloseAsync(await TryFallbackCaptureAsync());
        }
    }

    protected override void OnDisappearing()
    {
        if (_previewStarted)
        {
            PreviewCamera.StopCameraPreview();
        }

        if (!_finished)
        {
            CameraCaptureNavigation.Complete(null);
            _finished = true;
        }

        base.OnDisappearing();
    }

    private async void OnCaptureClicked(object? sender, EventArgs e)
    {
        if (_finished)
        {
            return;
        }

        CaptureButton.IsEnabled = false;

        try
        {
            var path = await CaptureFromPreviewAsync();
            await CloseAsync(path);
        }
        catch (Exception ex)
        {
            CaptureButton.IsEnabled = true;
            await DisplayAlert("Capture failed", ex.Message, "OK");
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async Task<string?> CaptureFromPreviewAsync()
    {
        _captureCompletion = new TaskCompletionSource<Stream?>(TaskCreationOptions.RunContinuationsAsynchronously);
        PreviewCamera.MediaCaptured += OnMediaCaptured;
        PreviewCamera.MediaCaptureFailed += OnMediaCaptureFailed;

        try
        {
            await PreviewCamera.CaptureImage(CancellationToken.None);
            var stream = await _captureCompletion.Task.WaitAsync(TimeSpan.FromSeconds(30));
            if (stream is null)
            {
                return null;
            }

            var localPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid():N}.jpg");
            await using var destination = File.OpenWrite(localPath);
            await stream.CopyToAsync(destination);
            return localPath;
        }
        finally
        {
            PreviewCamera.MediaCaptured -= OnMediaCaptured;
            PreviewCamera.MediaCaptureFailed -= OnMediaCaptureFailed;
            _captureCompletion = null;
        }
    }

    private void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
    {
        _captureCompletion?.TrySetResult(e.Media);
    }

    private void OnMediaCaptureFailed(object? sender, MediaCaptureFailedEventArgs e)
    {
        _captureCompletion?.TrySetException(
            new InvalidOperationException($"Camera capture failed: {e.FailureReason}"));
    }

    private static async Task<string?> TryFallbackCaptureAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            return null;
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "Take a food photo"
        });

        if (photo is null)
        {
            return null;
        }

        var localPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid():N}.jpg");
        await using var source = await photo.OpenReadAsync();
        await using var destination = File.OpenWrite(localPath);
        await source.CopyToAsync(destination);
        return localPath;
    }

    private async Task CloseAsync(string? path)
    {
        if (_finished)
        {
            return;
        }

        _finished = true;
        CameraCaptureNavigation.Complete(path);

        if (Shell.Current is not null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
