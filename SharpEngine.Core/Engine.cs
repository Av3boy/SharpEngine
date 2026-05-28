using Microsoft.Extensions.Logging;

using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Windowing;
using SharpEngine.Telemetry;

using System.Threading.Tasks;

namespace SharpEngine.Core;

/// <summary>
///     Manages the engine's services and provides methods to initialize, register handlers, and shut down asynchronously.
/// </summary>
public static class Engine
{
    /// <summary>
    ///     Gets the manager responsible for handling engine services.
    /// </summary>
    public static EngineServiceManager Services { get; private set; } = new();

    private static bool _initialized = false;

    private readonly static ILogger _logger;

    static Engine()
    {
        Initialize();
        _logger = LoggingExtensions.CreateLogger(typeof(Engine));
    }

    /// <summary>
    ///     Initializes the engine for use.
    /// </summary>
    public static void Initialize()
    {
        _logger.LogDebug("Initializing engine...");

        if (_initialized)
        {
            _logger.LogWarning("Reinitializing engine.");
            Services.StopAllAsync().Wait();
        }

        _initialized = true;
        _logger.LogDebug("Engine successfully initialized.");
    }

    /// <summary>
    ///     Creates and initializes a new window using the provided <see cref="Game"/> context and registers the window handler.
    /// </summary>
    /// <param name="game">The game context provides access to the current scene and camera settings for window initialization.</param>
    /// <returns>Returns the newly created <see cref="Window"/> instance.</returns>
    public static Window Initialize(Game game)
    {
        var window = new Window(game);

        Initialize();
        Services.RegisterHandler(new WindowHandler(window));

        return window;
    }

    /// <summary>
    ///     Stops all engine services and shuts down the engine asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when shutdown finishes.</returns>
    public static async Task ShutdownAsync()
    {
        if (Services == null)
            return;

        _logger.LogDebug("Shutting down engine...");

        await Services.StopAllAsync();

        _initialized = false;
        _logger.LogDebug("Engine successfully shut down.");
    }
}