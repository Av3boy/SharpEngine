using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEngine.Core.Handlers
{

    /// <summary>
    ///     Base class for long-running engine handlers that run asynchronously.
    /// </summary>
    /// <remarks>
    ///     Implementations should override <see cref="ExecuteAsync"/> to perform the handler's work. 
    ///     The handler can be started with <see cref="Start"/> and stopped with <see cref="StopAsync"/>. 
    ///     
    ///     Consumers can await <see cref="WaitForCompletionAsync"/> to observe when the handler has finished executing.
    /// </remarks>
    public abstract class EngineHandler : IAsyncDisposable
    {
        private Task? _runner;
        private CancellationTokenSource? _cts;
        private readonly TaskCompletionSource _completionSource = new();
        private DateTime _startTime;

        /// <summary>
        ///     Logger for recording diagnostic and operational messages for the handler.
        /// </summary>
        /// <remarks>
        ///     Provides structured, category-specific logging for EngineHandler and its derived classes.
        /// </remarks>
        protected readonly ILogger<EngineHandler> Logger;

        /// <summary>
        ///     Initializes a new instance of <see cref="EngineHandler"/>.
        /// </summary>
        /// <param name="logger">A logger for logging handler events.</param>
        protected EngineHandler(ILogger<EngineHandler> logger)
        {
            Logger = logger;
        }

        /// <summary>
        ///     Gets the current lifecycle state of the handler.
        /// </summary>
        public EngineHandlerState State { get; private set; } = EngineHandlerState.NotStarted;

        /// <summary>
        ///     Gets the total execution duration of the handler once it has stopped or faulted.
        /// </summary>
        public TimeSpan ExecutionDuration => State is EngineHandlerState.Stopped or EngineHandlerState.Faulted ?
            DateTime.UtcNow - _startTime : 
            TimeSpan.Zero;

        /// <summary>
        /// Starts the handler and begins executing <see cref="ExecuteAsync"/> on a background task.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the handler has already been started.</exception>
        public void Start()
        {
            if (State != EngineHandlerState.NotStarted)
                throw new InvalidOperationException("Handler already started.");

            _cts = new CancellationTokenSource();
            _startTime = DateTime.UtcNow;
            _runner = Task.Run(() => RunInternalAsync(_cts.Token));
        }

        /// <summary>
        ///     Requests cancellation and waits for the handler to stop.
        /// </summary>
        /// <remarks>
        ///     If the handler has not been started or is already stopped, this method returns immediately.
        /// </remarks>
        public async Task StopAsync()
        {
            if (_cts == null || _runner == null || State == EngineHandlerState.Stopped)
            {
                Logger.LogDebug("[EngineHandler] '{Name}' is not running.", GetType().Name);
                return;
            }

            await _cts.CancelAsync();

            try
            {
                await _runner;
            }
            catch (OperationCanceledException)
            {
                Logger.LogDebug("[EngineHandler] '{Name}' was canceled.", GetType().Name);
            }

            await OnStopAsync();
            State = EngineHandlerState.Stopped;

            Logger.LogDebug("[EngineHandler] '{Name}' stopped.", GetType().Name);
        }

        /// <summary>
        /// Asynchronously waits for the handler to complete execution, either by stopping normally,
        /// being cancelled, or faulting.
        /// </summary>
        /// <returns>A task that completes when the handler has finished.</returns>
        public async Task WaitForCompletionAsync() => await _completionSource.Task;

        /// <summary>
        ///     Executes the handler's work. Implementations should honor the provided
        ///     <paramref name="token"/> and return promptly when it signals cancellation.
        /// </summary>
        /// <param name="token">The cancellation token that is signaled when the handler should stop.</param>
        protected abstract Task ExecuteAsync(CancellationToken token);

        /// <summary>
        ///     Optional override called before the execution of the handler.
        /// </summary>
        protected virtual Task OnInitializedAsync() => Task.CompletedTask;

        /// <summary>
        ///     Optional override called after the handler has stopped or been cancelled.
        /// </summary>
        protected virtual Task OnStopAsync() => Task.CompletedTask;

        private async Task RunInternalAsync(CancellationToken token)
        {
            try
            {
                State = EngineHandlerState.Running;
                await OnInitializedAsync();
                await ExecuteAsync(token);
                State = EngineHandlerState.Stopped;
            }
            catch (OperationCanceledException)
            {
                State = EngineHandlerState.Stopped;
                Logger.LogDebug("[EngineHandler] '{Name}' was canceled.", GetType().Name);
            }
            catch (Exception ex)
            {
                State = EngineHandlerState.Faulted;
                Logger.LogError(ex, "[EngineHandler] Unhandled exception in '{Name}': {Exception}", GetType().Name, ex.Message);
            }
            finally
            {
                _completionSource.TrySetResult();
            }
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Disposes internal resources used by the handler. Implementations that
        ///     require asynchronous disposal should override this method.
        /// </remarks>
        public virtual ValueTask DisposeAsync()
        {
            _cts?.Dispose();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
    