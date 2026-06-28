using Microsoft.Extensions.Logging;

using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Telemetry;
using SharpEngine.Core.Windowing;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEngine.Core.Handlers;

/// <summary>
///     Manages application windows and their input contexts. 
///     
///     This handler maintains queue of window creation requests, starts a background task to process that queue,
///     and runs the per-frame update/render loop for all active windows until cancellation is requested.
/// </summary>
/// <remarks>
///     Windows are created and enqueued on a background task. 
///     
///     The handler will call DoEvents, DoUpdate and DoRender on each managed window every loop iteration.
///     When a window is closing it will be disposed and removed from the managed list.
/// </remarks>
public class WindowHandler : EngineHandler
{
    private static readonly List<SilkWindow> _windows = [];
    private static readonly List<IInputContext> _inputContexts = [];
    private static readonly ConcurrentQueue<WindowOptions> _windowQueue = [];
    private static readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly ILogger<WindowHandler> _logger;

    private SilkWindow? _mainWindow;

    /// <summary>
    ///     Gets the main window registered with this handler, if any.
    /// </summary>
    public SilkWindow? MainWindow => _mainWindow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowHandler"/>
    /// </summary>
    /// <remarks>
    ///     Starts the window queue as a background task.
    /// </remarks>
    /// <param name="logger">Optional logger instance; if null a default logger will be created.</param>
    public WindowHandler(ILogger<WindowHandler>? logger = null) : base(logger ?? LoggingExtensions.CreateLogger<WindowHandler>())
    {
        _logger = logger ?? LoggingExtensions.CreateLogger<WindowHandler>();

        // Enqueue a default window creation request when no explicit window is provided.
        StartWindowQueueTask(WindowOptions.Default);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowHandler"/>
    /// </summary>
    /// <remarks>
    ///     Starts the window queue as a background task.
    /// </remarks>
    /// <param name="window">An existing window instance.</param>
    /// <param name="logger">Optional logger instance; if null a default logger will be created.</param>
    public WindowHandler(SilkWindow window, ILogger<WindowHandler>? logger = null) : base(logger ?? LoggingExtensions.CreateLogger<WindowHandler>())
    {
        _logger = logger ?? LoggingExtensions.CreateLogger<WindowHandler>();

        // Register the provided window as the main window so it is managed immediately.
        RegisterWindow(window, isMain: true);

        // Start the queue task but do not enqueue an additional default window.
        StartWindowQueueTask(window.Settings.WindowOptions);
    }

    /// <summary>
    ///     Starts a background task that enqueues an initial <see cref="WindowOptions"/> and runs until the shared cancellation token is requested.
    /// </summary>
    /// <param name="options">Optional window options to enqueue; if null <see cref="WindowOptions.Default"/> is used.</param>
    private void StartWindowQueueTask(WindowOptions? options = null)
    {
        Task.Run(async () =>
        {
            // Only enqueue when an explicit options value is provided. The caller
            // controls whether a default window should be queued.
            if (options is not null)
                _windowQueue.Enqueue(options.Value);

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(1000);
                _logger.LogDebug("Running loop on background thread...");
            }
        });
    }

    /// <inheritdoc />
    /// <summary>
    ///     Runs the main update loop for managed windows. 
    /// </summary>
    /// <remarks>
    ///     This method will continue to run until the provided cancellation <paramref name="token"/> is cancelled.
    /// </remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that, when cancelled, ends the loop.</param>
    protected override Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < _windows.Count; i++)
                UpdateWindow(ref i);

            DequeueWindows();
        }

        return Task.FromCanceled(token);
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
    private static void UpdateWindow(ref int i)
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
    private static void DequeueWindows()
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            return;

        while (_windowQueue.TryDequeue(out var options))
            EnqueueWindow(options);
    }

    /// <summary>
    ///     Creates and initializes a new <see cref="Windowing.Window"/> instance using the provided <see cref="WindowOptions"/>. 
    ///     
    ///     The created window's position will be offset based on the current number of managed windows to help avoid overlap.
    /// </summary>
    /// <param name="options">Options to use when creating the window.</param>
    /// <returns>A newly initialized <see cref="Windowing.Window"/>.</returns>
    private static Windowing.Window CreateWindow(WindowOptions options)
    {
        var viewSettings = new DefaultViewSettings() with
        {
            WindowOptions = options with
            {
                Title = options.Title ?? "Window" + _windows.Count,

                // This is to make sure the windows don't overlap
                Position = new Vector2D<int>(
                    x: 500 + (50 * _windows.Count),
                    y: 400 + (50 * _windows.Count))
            }
        };

        var window = new Windowing.Window(new(), viewSettings, LoggingExtensions.CreateLogger<Windowing.Window>());
        window.Initialize();

        return window;
    }

    /// <summary>
    ///     Creates a new window from the provided options, wires up mouse click handlers, and begins managing the window and its input context.
    /// </summary>
    /// <param name="options">Options used to create the new window.</param>
    private static void EnqueueWindow(WindowOptions options)
    {
        var window = CreateWindow(options);
        // Subscribe to the window's high-level mouse button event instead of
        // attaching to the low-level Input.Mice click handlers. This centralizes
        // input handling inside Window and avoids duplicate wiring.
        window.OnButtonMouseDown += (mouse, button) => Mouse_Click(mouse, (Silk.NET.Input.MouseButton)button, mouse.Position);

        _inputContexts.Add(window.Input);
        _windows.Add(window);
    }

    /// <summary>
    ///     Registers an existing window instance with the handler and optionally marks it as the main window.
    /// </summary>
    /// <param name="window">The window instance to register.</param>
    /// <param name="isMain">Whether this window should be considered the main window.</param>
    private void RegisterWindow(SilkWindow window, bool isMain = false)
    {
        if (window is null)
            return;

        // Prefer the higher-level event on the Window class for mouse button
        // notifications rather than wiring Input.Mice click events directly.
        if (window is Windowing.Window w)
            w.OnButtonMouseDown += (mouse, button) => Mouse_Click(mouse, (Silk.NET.Input.MouseButton)button, mouse.Position);

        if (window.Input is not null)
            _inputContexts.Add(window.Input);

        _windows.Add(window);

        if (isMain)
            _mainWindow = window;
    }

    /// <summary>
    ///     Global mouse click handler that enqueues a request to create a new default window.
    /// </summary>
    /// <param name="args1">The mouse instance that raised the click event.</param>
    /// <param name="arg2">The mouse button that was clicked.</param>
    /// <param name="arg3">The pointer position when the click occurred.</param>
    private static void Mouse_Click(IMouse args1, Silk.NET.Input.MouseButton arg2, System.Numerics.Vector2 arg3)
    {
        _windowQueue.Enqueue(WindowOptions.Default);
    }
}
