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
    private CancellationTokenSource? _speechCts;

    public bool IsSpeaking { get; private set; }

    public HardwareService(ISpeechToText speechToText)
    {
        _speechToText = speechToText;
    }

    /// <summary>Opens the in-app camera preview; user taps Take photo when ready (never opens gallery).</summary>
    public Task<string?> TakePhotoAsync() => CameraCaptureNavigation.CaptureAsync();

    /// <summary>Opens gallery or file picker only (never opens camera).</summary>
    public async Task<string?> PickPhotoAsync() => await PickPhotoInternalAsync();

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

        StopSpeaking();
        _speechCts = new CancellationTokenSource();
        IsSpeaking = true;

        try
        {
#if WINDOWS
            await Platforms.Windows.WindowsTextToSpeech.SpeakAsync(text, _speechCts.Token);
#elif ANDROID
            await Platforms.Android.AndroidTextToSpeechHelper.SpeakAsync(text, _speechCts.Token);
#else
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                if (locales is null || !locales.Any())
                {
                    throw new FeatureNotSupportedException("No text-to-speech voice is installed on this device.");
                }

                var locale = locales.FirstOrDefault(l => l.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                    ?? locales.First();

                var options = new SpeechOptions
                {
                    Pitch = 1.0f,
                    Volume = 1.0f,
                    Locale = locale
                };

                await TextToSpeech.Default.SpeakAsync(text, options, _speechCts.Token);
            });
#endif
        }
        catch (TimeoutException)
        {
            StopSpeaking();
            throw new InvalidOperationException(
                "Reading timed out. Check that your device volume is up and text-to-speech is enabled.");
        }
        catch (OperationCanceledException)
        {
            // User stopped playback.
        }
        finally
        {
            IsSpeaking = false;
        }
    }

    public void StopSpeaking()
    {
#if WINDOWS
        Platforms.Windows.WindowsTextToSpeech.Stop();
#elif ANDROID
        Platforms.Android.AndroidTextToSpeechHelper.Stop();
#endif
        if (_speechCts is not null)
        {
            _speechCts.Cancel();
            _speechCts.Dispose();
            _speechCts = null;
        }

        IsSpeaking = false;
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

    private static async Task<string?> CapturePhotoInternalAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            throw new PermissionException("Camera permission was denied.");
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "Capture food photo"
        });

        return photo is null ? null : await SavePhotoAsync(photo);
    }

    private static async Task<string?> PickPhotoInternalAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select a food photo"
        });

        return photo is null ? null : await SavePhotoAsync(photo);
    }

    private static async Task<string> SavePhotoAsync(FileResult photo)
    {
        var localPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid():N}.jpg");
        await using var source = await photo.OpenReadAsync();
        await using var destination = File.OpenWrite(localPath);
        await source.CopyToAsync(destination);
        return localPath;
    }

    /// <summary>Copies a picked/captured image into app storage so it can be used as a recipe cover photo.</summary>
    public static async Task<string> SaveRecipeCoverImageAsync(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            throw new FileNotFoundException("The selected photo could not be found.", sourcePath);
        }

        var directory = Path.Combine(FileSystem.AppDataDirectory, "recipe_images");
        Directory.CreateDirectory(directory);
        var destinationPath = Path.Combine(directory, $"{Guid.NewGuid():N}.jpg");

        await using var source = File.OpenRead(sourcePath);
        await using var destination = File.Create(destinationPath);
        await source.CopyToAsync(destination);
        return destinationPath;
    }
}
