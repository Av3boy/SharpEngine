using Silk.NET.OpenAL;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Represents audio data in WAV format.
/// </summary>
public class WavData
{
    /// <summary>Gets or sets the number of channels used in the data.</summary>
    public short NumChannels { get; set; }

    /// <summary>Gets or sets the sample rate of the data.</summary>
    public int SampleRate { get; set; }

    /// <summary>Gets or sets the number of bits per sample.</summary>
    public short BitsPerSample { get; set; }

    /// <summary>Gets or sets the format of the buffer.</summary>
    /// <remarks>Defines how data is organized in the buffer.</remarks>
    public BufferFormat Format { get; set; }
}