using Silk.NET.OpenAL;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Represents a source for audio
/// </summary>
public class AudioSource
{
    private readonly uint _source;
    private readonly AL _al;

    /// <summary>
    ///     Initializes a new instance of <see cref="AudioSource"/>.
    /// </summary>
    /// <param name="al">An accessor to the OpenAL context.</param>
    public AudioSource(AL al)
    {
        _al = al;
        _source = _al.GenSource();
    }

    /// <summary>
    ///     Gets the source identifier.
    /// </summary>
    /// <returns>The identifier to the source.</returns>
    public uint Get() => _source;

    /// <summary>
    ///     Plays the given audio from the <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">A buffer containing the audio data.</param>
    public void Play(AudioBuffer buffer)
    {
        _al.SetSourceProperty(_source, SourceInteger.Buffer, buffer.BufferId);
        _al.SourcePlay(_source);
    }

    /// <summary>
    ///     Stops playing the audio.
    /// </summary>
    public void Stop()
    {
        _al.SourceStop(_source);
        _al.DeleteSource(_source);
    }
}