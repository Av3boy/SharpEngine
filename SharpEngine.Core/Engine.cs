using Microsoft.Extensions.Logging;

using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Windowing;
using SharpEngine.Shared.Dto;
using SharpEngine.Telemetry;

using System.Threading.Tasks;
using SharpEngine.Core.Extensions;

namespace SharpEngine.Core;

/// <summary>
///     Manages the engine's services and provides methods to initialize, register handlers, and shut down asynchronously.
/// </summary>
public class Engine
{
    /// <summary>
    ///     Gets the manager responsible for handling engine services.
    /// </summary>
    public EngineServiceManager ServicesManager { get; private set; } = new();

    private bool _initialized = false;
    private readonly ILogger _logger;

    public Engine()
    {
        _logger = LoggingExtensions.CreateLogger(typeof(Engine));
        Initialize();
    }

    /// <summary>
    ///     Initializes the engine for use.
    /// </summary>
    public void Initialize()
    {
        _logger.LogDebug("Initializing engine...");

        if (_initialized)
        {
            _logger.LogWarning("Reinitializing engine.");
            ServicesManager.StopAllAsync().Wait();
        }

        _initialized = true;
        _logger.LogDebug("Engine successfully initialized.");
    }

    /// <summary>
    ///     Creates and initializes a new window using the provided <see cref="Game"/> context and registers the window handler.
    /// </summary>
    /// <param name="game">The game context provides access to the current scene and camera settings for window initialization.</param>
    public void Initialize(Game game)
    {
        //var window = new Window(game);

        Initialize();
        ServicesManager.RegisterHandler(new WindowHandler());

        //return window;
    }

    /// <summary>
    ///     Stops all engine services and shuts down the engine asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when shutdown finishes.</returns>
    public async Task ShutdownAsync()
    {
        if (ServicesManager == null)
            return;

        _logger.LogDebug("Shutting down engine...");

        await ServicesManager.StopAllAsync();

        _initialized = false;
        _logger.LogDebug("Engine successfully shut down.");
    }

    public void CheckEngineVersion(ProjectDto project)
    {
        var currentAssemblyVersion = typeof(Window).Assembly.GetVersion();
        if (currentAssemblyVersion != project.EngineVersion.Version)
            _logger.LogWarning("The current engine version ({CurrentVersion}) does not match the project engine version ({ProjectVersion}). This may lead to unexpected behavior.", currentAssemblyVersion, project.EngineVersion);
    }
}