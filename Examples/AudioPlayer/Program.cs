using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Silk.NET.OpenAL;

namespace WavePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Exactly one argument should be given: the path to the .wav file that should be played.");
                return;
            }

            var filePath = args[0];
            var audioPlayer = new AudioPlayer();
            audioPlayer.Play(filePath);
        }
    }

    public class AudioPlayer
    {
        public void Play(string filePath)
        {
            ReadOnlySpan<byte> file = File.ReadAllBytes(filePath);
            int index = 0;
            if (file[index++] != 'R' || file[index++] != 'I' || file[index++] != 'F' || file[index++] != 'F')
            {
                Console.WriteLine("Given file is not in RIFF format");
                return;
            }

            var chunkSize = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
            index += 4;

            if (file[index++] != 'W' || file[index++] != 'A' || file[index++] != 'V' || file[index++] != 'E')
            {
                Console.WriteLine("Given file is not in WAVE format");
                return;
            }

            var wavData = new WavData();
            var audioDevice = new AudioDevice();
            var audioBuffer = new AudioBuffer(audioDevice.Al);
            var audioSource = new AudioSource(audioDevice.Al);

            while (index + 4 < file.Length)
            {
                var identifier = "" + (char)file[index++] + (char)file[index++] + (char)file[index++] + (char)file[index++];
                var size = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
                index += 4;
                if (identifier == "fmt ")
                {
                    wavData.ParseFormat(file.Slice(index, size));
                    index += size;
                }
                else if (identifier == "data")
                {
                    var data = file.Slice(index, size);
                    index += size;
                    audioBuffer.LoadData(data, wavData);
                }
                else
                {
                    index += size;
                }
            }

            audioSource.Play(audioBuffer);
            Console.WriteLine("Press Enter to Exit...");
            Console.ReadLine();
            audioSource.Stop();
        }
    }

    public class WavData
    {
        public short NumChannels { get; private set; }
        public int SampleRate { get; private set; }
        public short BitsPerSample { get; private set; }
        public BufferFormat Format { get; private set; }

        public void ParseFormat(ReadOnlySpan<byte> formatChunk)
        {
            int index = 0;
            var audioFormat = BinaryPrimitives.ReadInt16LittleEndian(formatChunk.Slice(index, 2));
            index += 2;
            if (audioFormat != 1)
            {
                throw new InvalidOperationException($"Unknown Audio Format with ID {audioFormat}");
            }

            NumChannels = BinaryPrimitives.ReadInt16LittleEndian(formatChunk.Slice(index, 2));
            index += 2;
            SampleRate = BinaryPrimitives.ReadInt32LittleEndian(formatChunk.Slice(index, 4));
            index += 4;
            index += 6; // Skip byteRate and blockAlign
            BitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(formatChunk.Slice(index, 2));
            index += 2;

            Format = NumChannels switch
            {
                1 => BitsPerSample == 8 ? BufferFormat.Mono8 : BufferFormat.Mono16,
                2 => BitsPerSample == 8 ? BufferFormat.Stereo8 : BufferFormat.Stereo16,
                _ => throw new InvalidOperationException($"Can't play audio with {NumChannels} channels")
            };
        }
    }

    public unsafe class AudioDevice : IDisposable
    {
        public AL Al { get; }
        private readonly ALContext _alc;
        private readonly Device* _device;
        private readonly Context* _context;

        public AudioDevice()
        {
            _alc = ALContext.GetApi();
            Al = AL.GetApi();
            _device = _alc.OpenDevice("");
            if (_device == null)
            {
                throw new InvalidOperationException("Could not create device");
            }

            _context = _alc.CreateContext(_device, null);
            _alc.MakeContextCurrent(_context);
            Al.GetError();
        }

        public void Dispose()
        {
            _alc.DestroyContext(_context);
            _alc.CloseDevice(_device);
            Al.Dispose();
            _alc.Dispose();
        }
    }

    public class AudioBuffer
    {
        private readonly uint _buffer;

        public AudioBuffer(AL al)
        {
            _buffer = al.GenBuffer();
        }

        public void LoadData(ReadOnlySpan<byte> data, WavData wavData)
        {
            unsafe
            {
                fixed (byte* pData = data)
                {
                    AL.GetApi().BufferData(_buffer, wavData.Format, pData, data.Length, wavData.SampleRate);
                }
            }
        }

        public uint BufferId => _buffer;
    }

    public class AudioSource
    {
        private readonly uint _source;

        public AudioSource(AL al)
        {
            _source = al.GenSource();
            al.SetSourceProperty(_source, SourceBoolean.Looping, true);
        }

        public void Play(AudioBuffer buffer)
        {
            AL.GetApi().SetSourceProperty(_source, SourceInteger.Buffer, buffer.BufferId);
            AL.GetApi().SourcePlay(_source);
        }

        public void Stop()
        {
            AL.GetApi().SourceStop(_source);
            AL.GetApi().DeleteSource(_source);
        }
    }
}
