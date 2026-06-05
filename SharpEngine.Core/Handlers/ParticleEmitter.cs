using Microsoft.Extensions.Logging;
using SharpEngine.Core.Entities;
using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

namespace SharpEngine.Core.Handlers;

internal class ParticleEmitter : EngineHandler
{
    public int EmissionRateMilliseconds { get; set; }
    public int ParticleLifeTimeMilliseconds { get; set; }

    private readonly List<Particle> _particles = new();
    private readonly object _particlesLock = new();

    private readonly SemaphoreSlim _pauseSemaphore = new(1, 1);
    private bool _isPaused = false;

    /// <summary>
    ///     Initializes a new instance of <see cref="ParticleEmitter"/>.
    /// </summary>
    /// <param name="logger">The logger to use for logging events.</param>
    public ParticleEmitter(ILogger<ParticleEmitter> logger) : base(logger) { }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        // Run disposer and emitter concurrently so particles are emitted and
        // disposed in parallel while respecting pause behavior.
        await Task.WhenAll(StartDisposer(token), StartEmitter(token));
    }

    private async Task StartEmitter(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Wait if emitting is paused
            await _pauseSemaphore.WaitAsync(token);
            _pauseSemaphore.Release();

            EmitParticle();

            // Wait for the next emission
            await Task.Delay(EmissionRateMilliseconds, token);
        }
    }

    private async Task StartDisposer(CancellationToken token)
    {
        // Periodically remove expired particles. This loop honors the same
        // pause semaphore so disposal pauses when emitter is paused.
        while (!token.IsCancellationRequested)
        {
            // Wait if emitting is paused
            await _pauseSemaphore.WaitAsync(token);
            _pauseSemaphore.Release();

            // Remove expired particles
            var nowTicks = DateTime.UtcNow.Ticks;
            lock (_particlesLock)
            {
                _particles.RemoveAll(p => (nowTicks - p.StartTimeTicks) / TimeSpan.TicksPerMillisecond >= p.LifeTimeMilliseconds);
            }

            // Avoid a tight loop — check disposal every 50ms
            await Task.Delay(50, token);
        }
    }

    private void EmitParticle()
    {
        var particle = new Particle(ParticleLifeTimeMilliseconds);
        lock (_particlesLock)
        {
            _particles.Add(particle);
        }
    }

    /// <summary>
    ///     Pauses the emission of particles.
    /// </summary>
    /// <remarks>
    ///     If already paused, this method does nothing.
    /// </remarks>
    public void Pause()
    {
        if (_isPaused)
            return;

        _isPaused = true;
        _pauseSemaphore.Wait();
    }

    /// <summary>
    ///     Resumes the emission of particles.
    /// </summary>
    /// <remarks>
    ///     If not paused, this method does nothing.
    /// </remarks>
    public void Resume()
    {
        if (!_isPaused)
            return;

        _pauseSemaphore.Release();
        _isPaused = false;
    }
}