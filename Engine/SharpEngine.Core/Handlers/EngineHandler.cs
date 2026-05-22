using SharpEngine.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class EngineHandler : IAsyncDisposable
{
    private Task? _runner;
    private CancellationTokenSource? _cts;
    private readonly TaskCompletionSource _completionSource = new();
    private DateTime _startTime;

    public EngineHandlerState State { get; private set; } = EngineHandlerState.NotStarted;
    public TimeSpan? ExecutionDuration => State is EngineHandlerState.Stopped or EngineHandlerState.Faulted ?
        DateTime.UtcNow - _startTime : null;

    public void Start()
    {
        if (State != EngineHandlerState.NotStarted)
            throw new InvalidOperationException("Handler already started.");

        _cts = new CancellationTokenSource();
        _startTime = DateTime.UtcNow;
        _runner = Task.Run(() => RunInternalAsync(_cts.Token));
    }

    public async Task StopAsync()
    {
        if (_cts == null || _runner == null || State == EngineHandlerState.Stopped)
            return;

        await _cts.CancelAsync();
        
        try
        {
            await _runner;
        }
        catch (OperationCanceledException)
        {
            Debug.Log.Information("[EngineHandler] '{Name}' was canceled.", GetType().Name);
        }

        await OnStopAsync();
        State = EngineHandlerState.Stopped;
    }

    public async Task WaitForCompletionAsync() => await _completionSource.Task;

    protected abstract Task ExecuteAsync(CancellationToken token);

    protected virtual Task OnStartAsync() => Task.CompletedTask;
    protected virtual Task OnStopAsync() => Task.CompletedTask;

    private async Task RunInternalAsync(CancellationToken token)
    {
        try
        {
            State = EngineHandlerState.Running;
            await OnStartAsync();
            await ExecuteAsync(token);
            State = EngineHandlerState.Stopped;
        }
        catch (OperationCanceledException)
        {
            State = EngineHandlerState.Stopped;
        }
        catch (Exception ex)
        {
            State = EngineHandlerState.Faulted;
            Debug.Log.Error(ex, "[EngineHandler] Unhandled exception in '{Name}': {Exception}", GetType().Name, ex.Message);
        }
        finally
        {
            _completionSource.TrySetResult();
        }
    }

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
