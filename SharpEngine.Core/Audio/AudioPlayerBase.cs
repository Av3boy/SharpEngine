using SharpEngine.Core.Scenes;

using System;
using System.IO;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Represents a base class for all audio players.
/// </summary>
public abstract class AudioPlayerBase : SceneNode
{
    /// <summary>Gets the extension accepted by the audio player.</summary>
    protected abstract string FileExtension { get; }

    /// <summary>Gets or sets the properties for the player.</summary>
    public AudioProperties AudioProperties { get; set; }

    /// <summary>Contains the audio data of the media file.</summary>
    protected readonly WavData Data;

    /// <summary>Represents a container for the <see cref="Data"/>.</summary>
    protected readonly AudioBuffer AudioBuffer;

    /// <summary>The source of the audio.</summary>
    protected readonly AudioSource AudioSource;

    /// <summary>
    ///     Initializes a new instance of <see cref="AudioPlayerBase"/>.
    /// </summary>
    protected AudioPlayerBase()
    {
        Data = new WavData();

        var audioDevice = new AudioDevice();
        AudioBuffer = new AudioBuffer(audioDevice.Al);
        AudioSource = new AudioSource(audioDevice.Al);

        AudioProperties = new AudioProperties(audioDevice.Al, AudioSource);
    }

    /// <summary>
    ///     Plays a media file specified by the given path.
    /// </summary>
    /// <param name="filePath">The location of the media file to be played.</param>
    public abstract void Play(string filePath);

    /// <summary>
    ///     Validates the specified file by checking its extension and existence.
    /// </summary>
    /// <param name="filePath">The path to the file that needs to be validated for its type and existence.</param>
    /// <exception cref="ArgumentException">Thrown when the file does not have the allowed extension.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist in the file system.</exception>
    protected virtual void ValidateFile(string filePath)
    {
        if (Path.GetExtension(filePath) != FileExtension)
            throw new ArgumentException($"Given file is not a {FileExtension} file", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Given file does not exist", filePath);
    }

    /// <summary>
    ///     Stops the audio playback of the associated AudioSource.
    /// </summary>
    public virtual void Stop() => AudioSource.Stop();
}
