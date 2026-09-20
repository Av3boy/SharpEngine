using Silk.NET.OpenAL;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Contains properties used to control how the audio should be played.
/// </summary>
public class AudioProperties
{
    private readonly AL _al;
    private readonly uint _audioSourceId;

    /// <summary>
    ///     Initializes a new instance of <see cref="AudioProperties"/>.
    /// </summary>
    /// <param name="al">The context where the audio should be played.</param>
    /// <param name="audioSource">The source at which the audio should be played.</param>
    public AudioProperties(AL al, AudioSource audioSource)
    {
        _al = al;
        _audioSourceId = audioSource.Get();
    }

    /// <summary>Gets or sets whether the audio should be looping.</summary>
    public bool IsLooping
    {
        get => _isLooping;
        set
        {
            _isLooping = value;
            _al.SetSourceProperty(_audioSourceId, SourceBoolean.Looping, value);
        }
    }

    private bool _isLooping;
}
