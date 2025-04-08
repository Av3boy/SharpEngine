using Silk.NET.OpenAL;
using System;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Represents an audio buffer with a unique identifier.
/// </summary>
public class AudioBuffer
{
    /// <summary>Gets the identifier to the audio buffer.</summary>
    public uint BufferId => _buffer;

    private readonly uint _buffer;
    private readonly AL _al;

    /// <summary>
    ///     Initializes a new instance of <see cref="AudioBuffer"/>.
    /// </summary>
    /// <param name="al">The OpenAL API context.</param>
    public AudioBuffer(AL al)
    {
        _al = al;
        _buffer = _al.GenBuffer();
    }

    /// <summary>
    ///     Loads the given <paramref name="data"/> into the buffer.
    /// </summary>
    /// <param name="data">The data to be loaded into the buffer.</param>
    /// <param name="wavData">Details about the data.</param>
    public void LoadData(ReadOnlySpan<byte> data, WavData wavData)
    {
        unsafe
        {
            fixed (byte* pData = data)
            {
                _al.BufferData(_buffer, wavData.Format, pData, data.Length, wavData.SampleRate);
            }
        }
    }

}