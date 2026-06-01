#if ANDROID

using Android.Content;
using Android.Media;
using Android.OS;
using Android.Speech.Tts;
using Java.IO;
using Java.Lang;
using Java.Util;
using Microsoft.Maui.ApplicationModel;
using AndroidTts = Android.Speech.Tts.TextToSpeech;
using JavaFile = Java.IO.File;
using JavaLocale = Java.Util.Locale;

namespace FoodDrinkApp.Platforms.Android;

/// <summary>
/// Synthesizes speech to a WAV file, then plays it with MediaPlayer (works better on emulators than direct Speak).
/// </summary>
internal static class AndroidTextToSpeechHelper
{
    private const string UtteranceId = "FoodDrinkAppTts";
    private static MediaPlayer? _player;

    public static async Task SpeakAsync(string text, CancellationToken cancellationToken)
    {
        Stop();

        var context = Platform.AppContext
            ?? throw new InvalidOperationException("Android application context is not available.");

        var audioPath = await SynthesizeToFileAsync(context, text, cancellationToken);
        await PlayFileAsync(context, audioPath, cancellationToken);
    }

    public static void Stop()
    {
        if (_player is null)
        {
            return;
        }

        try
        {
            if (_player.IsPlaying)
            {
                _player.Stop();
            }

            _player.Release();
        }
        catch
        {
            // Ignore cleanup errors.
        }
        finally
        {
            _player = null;
        }
    }

    private static async Task<string> SynthesizeToFileAsync(
        Context context,
        string text,
        CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        AndroidTts? engine = null;
        var outputFile = new JavaFile(context.CacheDir, $"tts_{Guid.NewGuid():N}.wav");

        engine = new AndroidTts(context, new InitListener(success =>
        {
            if (engine is null)
            {
                return;
            }

            if (!success)
            {
                completion.TrySetException(new FeatureNotSupportedException(
                    "Text-to-speech failed to start. Install Google Text-to-speech in the emulator Play Store, " +
                    "then enable it under Settings → System → Languages → Text-to-speech output."));
                ShutdownEngine(engine);
                return;
            }

            engine.SetSpeechRate(0.95f);
            engine.SetPitch(1.0f);

            var languageResult = engine.SetLanguage(JavaLocale.English);
            if ((int)languageResult < 0)
            {
                engine.SetLanguage(JavaLocale.Default);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var attributes = new AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Media)
                    .SetContentType(AudioContentType.Speech)
                    .Build();
                engine.SetAudioAttributes(attributes);
            }

            engine.SetOnUtteranceProgressListener(new ProgressListener(
                utteranceId =>
                {
                    if (utteranceId != UtteranceId)
                    {
                        return;
                    }

                    if (!outputFile.Exists() || outputFile.Length() == 0)
                    {
                        completion.TrySetException(new InvalidOperationException(
                            "Speech file was not created. Install Google Text-to-speech in the emulator."));
                        ShutdownEngine(engine);
                        return;
                    }

                    completion.TrySetResult(outputFile.AbsolutePath!);
                    ShutdownEngine(engine);
                },
                utteranceId =>
                {
                    if (utteranceId == UtteranceId)
                    {
                        completion.TrySetException(new InvalidOperationException("Speech synthesis failed."));
                        ShutdownEngine(engine);
                    }
                }));

            var synthResult = engine.SynthesizeToFile(text, null, outputFile, UtteranceId);
            if (synthResult != OperationResult.Success)
            {
                completion.TrySetException(new InvalidOperationException("Text-to-speech engine rejected synthesis."));
                ShutdownEngine(engine);
            }
        }));

        using var registration = cancellationToken.Register(() =>
        {
            engine?.Stop();
            ShutdownEngine(engine);
            completion.TrySetCanceled();
        });

        var timeout = TimeSpan.FromSeconds(System.Math.Clamp(text.Length / 8 + 20, 25, 180));
        return await completion.Task.WaitAsync(timeout, cancellationToken);
    }

    private static async Task PlayFileAsync(Context context, string path, CancellationToken cancellationToken)
    {
        EnsureMediaVolume(context);

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _player = new MediaPlayer();
            _player.SetAudioAttributes(new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Speech)
                .Build());

            _player.Completion += (_, _) =>
            {
                Stop();
                TryDeleteFile(path);
                completion.TrySetResult();
            };

            _player.Error += (_, args) =>
            {
                Stop();
                TryDeleteFile(path);
                completion.TrySetException(new InvalidOperationException(
                    $"Audio playback failed (code {(int)args.What}). Turn up Media volume on the emulator."));
            };

            _player.SetDataSource(path);
            _player.Prepare();
            _player.Start();
        });

        using var registration = cancellationToken.Register(() =>
        {
            Stop();
            TryDeleteFile(path);
            completion.TrySetCanceled();
        });

        await completion.Task.WaitAsync(TimeSpan.FromSeconds(180), cancellationToken);
    }

    private static void EnsureMediaVolume(Context context)
    {
        var audioManager = (AudioManager?)context.GetSystemService(Context.AudioService);
        if (audioManager is null)
        {
            return;
        }

        var maxVolume = audioManager.GetStreamMaxVolume(global::Android.Media.Stream.Music);
        var currentVolume = audioManager.GetStreamVolume(global::Android.Media.Stream.Music);
        if (currentVolume < maxVolume)
        {
            audioManager.SetStreamVolume(global::Android.Media.Stream.Music, maxVolume, 0);
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            var file = new JavaFile(path);
            if (file.Exists())
            {
                file.Delete();
            }
        }
        catch
        {
            // Ignore cleanup errors.
        }
    }

    private static void ShutdownEngine(AndroidTts? engine)
    {
        if (engine is null)
        {
            return;
        }

        engine.Stop();
        engine.Shutdown();
        engine.Dispose();
    }

    private sealed class InitListener : Java.Lang.Object, AndroidTts.IOnInitListener
    {
        private readonly Action<bool> _onInit;

        public InitListener(Action<bool> onInit) => _onInit = onInit;

        public void OnInit(OperationResult status) => _onInit(status == OperationResult.Success);
    }

    private sealed class ProgressListener : UtteranceProgressListener
    {
        private readonly Action<string?> _onDone;
        private readonly Action<string?> _onError;

        public ProgressListener(Action<string?> onDone, Action<string?> onError)
        {
            _onDone = onDone;
            _onError = onError;
        }

        public override void OnStart(string? utteranceId)
        {
        }

        public override void OnDone(string? utteranceId) => _onDone(utteranceId);

#pragma warning disable CS0672, CS0618
        public override void OnError(string? utteranceId) => _onError(utteranceId);

        public override void OnError(string? utteranceId, TextToSpeechError errorCode) => _onError(utteranceId);
#pragma warning restore CS0672, CS0618
    }
}

#endif
