using System;
using System.IO;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Handles Audio related operations.
/// </summary>
public static class Audio
{
    private static AudioPlayerBase? audioPlayer = null;

    /// <summary>
    ///     Plays an audio file if it is in the WAV format.
    /// </summary>
    /// <param name="filePath">Specifies the location of the audio file to be played.</param>
    /// <exception cref="NotSupportedException">Thrown when the file format is not supported, such as when it is not a WAV file.</exception>
    public static void Play(string filePath)
    {
        switch (Path.GetExtension(filePath))
        {
            case ".wav":
                audioPlayer = new WavPlayer();
                audioPlayer.Play(filePath);
                break;

            case ".mp3":
                audioPlayer = new Mp3Player();
                audioPlayer.Play(filePath);
                break;

            default:
                throw new NotSupportedException("Unsupported file format.");
        }
    }

    /// <inheritdoc cref="AudioPlayerBase.Stop"/>
    public static void Stop()
    {
        audioPlayer?.Stop();
        audioPlayer = null;
    }

    /// <summary>
    ///     Pauses the currently playing audio, if any.
    /// </summary>
    public static void Pause() => audioPlayer?.Pause();

    /// <summary>
    ///     Resumes the currently paused audio, if any.
    /// </summary>
    public static void Resume() => audioPlayer?.Resume();
}
