#if WINDOWS

using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;

namespace FoodDrinkApp.Platforms.Windows;

/// <summary>
/// Reliable text-to-speech on Windows (MAUI TextToSpeech is unreliable on WinUI).
/// </summary>
internal static class WindowsTextToSpeech
{
    private static MediaPlayer? _player;
    private static SpeechSynthesizer? _synthesizer;
    private static IRandomAccessStream? _stream;
    private static TaskCompletionSource<bool>? _playbackCompletion;

    public static async Task SpeakAsync(string text, CancellationToken cancellationToken)
    {
        Stop();
        cancellationToken.ThrowIfCancellationRequested();

        _synthesizer = new SpeechSynthesizer();
        var speechStream = await _synthesizer.SynthesizeTextToStreamAsync(text);
        _stream = speechStream;

        _playbackCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _player = new MediaPlayer
        {
            AudioCategory = MediaPlayerAudioCategory.Speech,
            Volume = 1.0,
            IsMuted = false
        };

        _player.MediaEnded += OnMediaEnded;
        _player.MediaFailed += OnMediaFailed;
        _player.Source = MediaSource.CreateFromStream(speechStream, speechStream.ContentType);
        _player.Play();

        using var registration = cancellationToken.Register(Stop);

        var timeout = TimeSpan.FromSeconds(Math.Clamp(text.Length / 10.0 + 5.0, 8.0, 120.0));
        await _playbackCompletion.Task.WaitAsync(timeout, cancellationToken);
    }

    public static void Stop()
    {
        if (_player is not null)
        {
            _player.MediaEnded -= OnMediaEnded;
            _player.MediaFailed -= OnMediaFailed;
            _player.Pause();
            _player.Source = null;
            _player = null;
        }

        _stream?.Dispose();
        _stream = null;
        _synthesizer?.Dispose();
        _synthesizer = null;

        _playbackCompletion?.TrySetCanceled();
        _playbackCompletion = null;
    }

    private static void OnMediaEnded(MediaPlayer sender, object args)
    {
        _playbackCompletion?.TrySetResult(true);
    }

    private static void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        _playbackCompletion?.TrySetException(
            new InvalidOperationException($"Speech playback failed: {args.ErrorMessage}"));
    }
}

#endif
