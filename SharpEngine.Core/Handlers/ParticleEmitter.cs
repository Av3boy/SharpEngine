using SharpEngine.Core.Entities;
using SharpEngine.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEngine.Core.Handlers;

internal class ParticleEmitter : EngineHandler
{
    public int EmissionRateMilliseconds { get; set; }
    public int ParticleLifeTimeMilliseconds { get; set; }

    private readonly List<Particle> _particles = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly SemaphoreSlim _pauseSemaphore = new(1, 1);
    private bool _isPaused = false;

    internal ParticleEmitter()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => StartEmitter(_cancellationTokenSource.Token));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        await StartDisposer(token);
        await StartEmitter(token);
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
        while (!token.IsCancellationRequested)
        {
            // Wait if emitting is paused
            await _pauseSemaphore.WaitAsync(token);
            _pauseSemaphore.Release();
        }
    }

    public void EmitParticle()
    {
        var particle = new Particle(ParticleLifeTimeMilliseconds);
        _particles.Add(particle);

        Debug.Log.Information("Emitting particle.");
    }

    public void Pause()
    {
        if (_isPaused)
            return;

        _isPaused = true;
        _pauseSemaphore.Wait();
    }

    public void Resume()
    {
        if (!_isPaused)
            return;

        _pauseSemaphore.Release();
        _isPaused = false;
    }

    public void Stop() => _cancellationTokenSource.Cancel();

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Dispose();
        return base.DisposeAsync();
    }
}