using System;
using System.IO;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Handles Audio related operations.
/// </summary>
public static class Audio
{
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
                new WavPlayer().Play(filePath);
                break;

            default:
                throw new NotSupportedException("Unsupported file format.");
        }
    }
}
