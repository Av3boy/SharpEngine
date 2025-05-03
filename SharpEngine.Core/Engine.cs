using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Windowing;
using SharpEngine.Shared;

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

    static Engine()
    {
        Initialize();
    }

    /// <summary>
    ///     Initializes the engine for use.
    /// </summary>
    public static void Initialize()
    {
        Debug.Log.Debug("Initializing engine...");

        if (_initialized)
        {
            Debug.Log.Warning("Reinitializing engine.");
            Services.StopAllAsync().Wait();
        }

        Services = new EngineServiceManager();

        _initialized = true;
        Debug.Log.Debug("Engine successfully initialized.");
    }

    /// <summary>
    ///     
    /// </summary>
    /// <param name="game">The game context provides access to the current scene and camera settings for window initialization.</param>
    /// <returns>Returns the newly created window instance.</returns>
    public static Window Initialize(Game game)
    {
        var window = new Window(game);

        Initialize();
        Services.RegisterHandler(new WindowHandler(window));

        return window;
    }

    /// <summary>
    ///     Stops and clears all running services.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public static async Task ShutdownAsync()
    {
        if (Services == null)
            return;

        Debug.Log.Debug("Shutting down engine...");

        await Services.StopAllAsync();
        Services = new();

        _initialized = false;
        Debug.Log.Debug("Engine successfully shut down.");
    }
}