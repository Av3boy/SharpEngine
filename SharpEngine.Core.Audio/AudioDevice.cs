using Silk.NET.OpenAL;
using System;

namespace SharpEngine.Core.Audio;

/// <summary>
///     Manages an audio device and its context for audio operations.
/// </summary>
public unsafe class AudioDevice : IDisposable
{
    // Shared single AudioDevice instance to ensure a single OpenAL context is used
    private static readonly AudioDevice _shared = new AudioDevice();

    /// <summary>
    ///     Gets the shared audio device instance used by the application.
    /// </summary>
    public static AudioDevice Shared => _shared;

    /// <summary>Gets the OpenAL context.</summary>
    public AL Al { get; }

    private readonly ALContext _alc;
    private readonly Device* _device;
    private readonly Context* _context;

    /// <summary>
    ///     Initializes a new instance of <see cref="AudioDevice"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when unable to create the audio device.</exception>
    public AudioDevice()
    {
        _alc = ALContext.GetApi();
        Al = AL.GetApi();
        _device = _alc.OpenDevice("");
        
        if (_device == null)
            throw new InvalidOperationException("Could not create device.");

        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
        Al.GetError();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _alc.DestroyContext(_context);
        _alc.CloseDevice(_device);
        Al.Dispose();
        _alc.Dispose();

        GC.SuppressFinalize(this);
    }
}
