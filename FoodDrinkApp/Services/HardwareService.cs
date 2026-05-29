using System.Globalization;
using CommunityToolkit.Maui.Media;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;

namespace FoodDrinkApp.Services;

/// <summary>
/// Centralizes mobile hardware access for camera, location, speech, TTS, and haptics.
/// </summary>
public class HardwareService
{
    private readonly ISpeechToText _speechToText;

    public HardwareService(ISpeechToText speechToText)
    {
        _speechToText = speechToText;
    }

    public async Task<string?> TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            throw new FeatureNotSupportedException("Camera capture is not supported on this device.");
        }

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            throw new PermissionException("Camera permission was denied.");
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "Capture food photo"
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

    public async Task<Location?> GetCurrentLocationAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            throw new PermissionException("Location permission was denied.");
        }

        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
        return await Geolocation.Default.GetLocationAsync(request);
    }

    public async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("There is no text to read aloud.", nameof(text));
        }

        await TextToSpeech.Default.SpeakAsync(text);
    }

    public void Vibrate()
    {
        if (Vibration.Default.IsSupported)
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
            return;
        }

        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }

    public async Task<string?> RecognizeSpeechAsync(CancellationToken cancellationToken = default)
    {
        var isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            throw new PermissionException("Microphone or speech recognition permission was denied.");
        }

        var completionSource = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnRecognitionCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
        {
            _speechToText.RecognitionResultCompleted -= OnRecognitionCompleted;
            if (e.RecognitionResult.Exception is not null)
            {
                completionSource.TrySetException(e.RecognitionResult.Exception);
                return;
            }

            completionSource.TrySetResult(e.RecognitionResult.Text);
        }

        _speechToText.RecognitionResultCompleted += OnRecognitionCompleted;

        try
        {
            await _speechToText.StartListenAsync(
                new SpeechToTextOptions
                {
                    Culture = CultureInfo.CurrentCulture,
                    ShouldReportPartialResults = false
                },
                cancellationToken);

            using var listenWindow = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            listenWindow.CancelAfter(TimeSpan.FromSeconds(6));
            await Task.Delay(TimeSpan.FromSeconds(6), listenWindow.Token);
            await _speechToText.StopListenAsync(cancellationToken);

            var text = await completionSource.Task.WaitAsync(TimeSpan.FromSeconds(3), cancellationToken);
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        finally
        {
            _speechToText.RecognitionResultCompleted -= OnRecognitionCompleted;
        }
    }
}
