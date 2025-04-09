using Silk.NET.OpenAL;

using System;
using System.Buffers.Binary;
using System.IO;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Represents a player for .wav files.
/// </summary>
public class WavPlayer : AudioPlayerBase
{
    /// <inheritdoc />
    protected override string FileExtension => ".wav";

    private static class WavConstants
    {
        public static ReadOnlySpan<byte> RiffHeader => "RIFF"u8;
        public static ReadOnlySpan<byte> WaveHeader => "WAVE"u8;
        public static ReadOnlySpan<byte> FmtHeader => "fmt "u8;
        public static ReadOnlySpan<byte> DataHeader => "data"u8;

        public const int ChunkSizeLength = 4;
        public const int HeaderLength = 4;
        public const int FormatChunkSkipLength = 6;
        public const int AudioFormatLength = 2;
        public const int NumChannelsLength = 2;
        public const int SampleRateLength = 4;
        public const int BitsPerSampleLength = 2;
    }

    /// <inheritdoc />
    public override void Play(string filePath)
    {
        ValidateFile(filePath);

        ReadOnlySpan<byte> file = File.ReadAllBytes(filePath);
        int index = 0;

        if (!CheckHeader(file, ref index, WavConstants.RiffHeader))
        {
            Console.WriteLine("Given file is not in RIFF format");
            return;
        }

        index += WavConstants.ChunkSizeLength; // Skip chunk size

        if (!CheckHeader(file, ref index, WavConstants.WaveHeader))
        {
            Console.WriteLine("Given file is not in WAVE format");
            return;
        }

        while (index + WavConstants.HeaderLength <= file.Length)
        {
            var identifier = file.Slice(index, WavConstants.HeaderLength);
            index += WavConstants.HeaderLength;

            var size = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, WavConstants.ChunkSizeLength));
            index += WavConstants.ChunkSizeLength;

            var data = file.Slice(index, size);
            switch (identifier)
            {
                case var id when id.SequenceEqual(WavConstants.FmtHeader):
                    ParseFormat(data, Data);
                    break;

                case var id when id.SequenceEqual(WavConstants.DataHeader):
                    AudioBuffer.LoadData(data, Data);
                    break;
            }

            index += size;
        }

        AudioSource.Play(AudioBuffer);
    }

    private static bool CheckHeader(ReadOnlySpan<byte> file, ref int index, ReadOnlySpan<byte> header)
    {
        if (!file.Slice(index, header.Length).SequenceEqual(header))
            return false;

        index += header.Length;
        return true;
    }

    private static void ParseFormat(ReadOnlySpan<byte> formatChunk, WavData wavData)
    {
        int index = 0;
        var audioFormat = BinaryPrimitives.ReadInt16LittleEndian(formatChunk.Slice(index, WavConstants.AudioFormatLength));
        index += WavConstants.AudioFormatLength;

        if (audioFormat != 1)
            throw new InvalidOperationException($"Unknown Audio Format with ID {audioFormat}");

        wavData.NumChannels = BinaryPrimitives.ReadInt16LittleEndian(formatChunk.Slice(index, WavConstants.NumChannelsLength));
        index += WavConstants.NumChannelsLength;

        wavData.SampleRate = BinaryPrimitives.ReadInt32LittleEndian(formatChunk.Slice(index, WavConstants.SampleRateLength));
        index += WavConstants.SampleRateLength;
        
        index += WavConstants.FormatChunkSkipLength; // Skip byteRate and blockAlign
        wavData.BitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(formatChunk.Slice(index, WavConstants.BitsPerSampleLength));

        wavData.Format = wavData.NumChannels switch
        {
            1 => wavData.BitsPerSample == 8 ? BufferFormat.Mono8 : BufferFormat.Mono16,
            2 => wavData.BitsPerSample == 8 ? BufferFormat.Stereo8 : BufferFormat.Stereo16,
            _ => throw new InvalidOperationException($"Can't play audio with {wavData.NumChannels} channels")
        };
    }
}
