using Microsoft.Extensions.Logging;

using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Telemetry;
using SharpEngine.Core.Windowing;

using Silk.NET.Input;
using Silk.NET.Maths;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace SharpEngine.Core.Handlers;

/// <summary>
///     Manages application windows and their input contexts. 
///     
///     This handler maintains queue of window creation requests, starts a background task to process that queue,
///     and runs the per-frame update/render loop for all active windows until cancellation is requested.
/// </summary>
/// <remarks>
/// <list type="bullet">
///     <item>Windows are created and enqueued on a background task.</item>
///     <item>When a window is closing it will be disposed and removed from the managed list.</item>
/// </list>
/// </remarks>
public class WindowHandler : EngineHandler
{
    private readonly List<SilkWindow> _windows = [];
    private readonly List<IInputContext> _inputContexts = [];
    private readonly ConcurrentQueue<SilkWindow> _windowQueue = [];

    /// <summary>
    ///     Gets the main window registered with this handler, if any.
    /// </summary>
    public SilkWindow? MainWindow { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowHandler"/>
    /// </summary>
    /// <remarks>
    ///     Starts the window queue as a background task.
    /// </remarks>
    /// <param name="logger">Optional logger instance; if null a default logger will be created.</param>
    public WindowHandler(ILogger<WindowHandler>? logger = null) : base(logger) { }

    /// <inheritdoc />
    /// <summary>
    ///     Runs the main update loop for managed windows. 
    /// </summary>
    /// <remarks>
    ///     This method will continue to run until the provided cancellation <paramref name="token"/> is cancelled.
    /// </remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that, when cancelled, ends the loop.</param>
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < _windows.Count; i++)
                UpdateWindow(ref i);

            DequeueWindows();
        }

        foreach (var window in _windows)
            window.Close();

        await StopAsync();
    }

    /// <summary>
    ///     Advances a single window through its event, update and render phases.
    /// </summary>
    /// <remarks>
    ///     If the window is closing it will be disposed and removed from the managed list.
    /// </remarks>
    /// <param name="i">
    ///     Reference to the index of the window in the internal list; 
    ///     this method may decrement the index if the window is removed.
    /// </param>
    private void UpdateWindow(ref int i)
    {
        var window = _windows[i];
        if (window is null)
            return;

        window.DoEvents();
        window.DoUpdate();
        window.DoRender();

        if (window.IsClosing)
        {
            window.Reset();
            window.Dispose();

            _windows.RemoveAt(i);
            i--; // Adjust the index to account for the removed item

            if (_windows.Count == 0)
                _cancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    ///     Dequeues any pending window creation requests and enqueues the resulting windows for management. 
    ///     No action is taken if cancellation has been requested.
    /// </summary>
    private void DequeueWindows()
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            return;

        while (_windowQueue.TryDequeue(out var window))
        {
            window.Initialize();
            window.Run();

            _windows.Add(window);
        }
    }

    // TODO: Instead of enqueing the options, the entire window object should be added to the queue.
    /// <summary>
    ///     Registers an existing window instance with the handler and optionally marks it as the main window.
    /// </summary>
    /// <param name="window">The window instance to register.</param>
    /// <param name="isMain">Whether this window should be considered the main window.</param>
    public void AddWindow(SilkWindow window, bool isMain = false)
    {
        ArgumentNullException.ThrowIfNull(window, nameof(window));

        _windowQueue.Enqueue(window);

        if (isMain)
            MainWindow = window;
    }
}
