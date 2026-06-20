using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Binary;
using System.IO;
using NLayer;
using Silk.NET.OpenAL;

namespace SharpEngine.Core.Audio;

internal class Mp3Player : AudioPlayerBase
{
    private static readonly ILogger<Mp3Player> Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Mp3Player>();

    /// <inheritdoc />
    protected override string FileExtension => AudioFileExtensions.Mp3Extension;

    public Mp3Player()
    {
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown if the audio data contains more than 2 channels.</exception>
    /// <exception cref="ArgumentException">Thrown if the file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file is not found.</exception>
    public override void Play(string filePath)
    {
        ValidateFile(filePath);

        using var fs = File.OpenRead(filePath);
        using var mpeg = new MpegFile(fs);

        int sampleRate = mpeg.SampleRate;
        int channels = mpeg.Channels;

        // Decode to float samples and convert to 16-bit PCM
        using var pcmStream = new MemoryStream();
        var floatBuffer = new float[4096];
        int samplesRead;
        while ((samplesRead = mpeg.ReadSamples(floatBuffer, 0, floatBuffer.Length)) > 0)
        {
            // samplesRead is number of float samples (interleaved channels)
            var bytes = new byte[samplesRead * 2];
            for (int i = 0; i < samplesRead; i++)
            {
                float f = floatBuffer[i];
                // clamp and convert
                int intVal = (int)MathF.Round(f * 32767f);
                if (intVal > short.MaxValue) intVal = short.MaxValue;
                else if (intVal < short.MinValue) intVal = short.MinValue;
                short s = (short)intVal;
                bytes[i * 2] = (byte)(s & 0xFF);
                bytes[i * 2 + 1] = (byte)((s >> 8) & 0xFF);
            }

            pcmStream.Write(bytes, 0, bytes.Length);
        }

        var pcmData = pcmStream.ToArray();

        // Fill WavData
        Data.NumChannels = (short)channels;
        Data.SampleRate = sampleRate;
        Data.BitsPerSample = 16;
        Data.Format = channels switch
        {
            1 => BufferFormat.Mono16,
            2 => BufferFormat.Stereo16,
            _ => throw new InvalidOperationException($"Can't play audio with {channels} channels.")
        };

        AudioBuffer.LoadData(pcmData, Data);
        AudioSource.Play(AudioBuffer);
    }
}