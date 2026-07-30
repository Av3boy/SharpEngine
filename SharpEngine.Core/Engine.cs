using Microsoft.Extensions.Logging;

using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Windowing;
using SharpEngine.Shared.Dto;
using SharpEngine.Telemetry;

using System.Threading.Tasks;
using SharpEngine.Core.Extensions;
using System.Threading;

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
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Engine()
    {
        _logger = LoggingExtensions.CreateLogger(typeof(Engine));
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

        ServicesManager.StartHandlers(_cancellationTokenSource);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {

        }
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