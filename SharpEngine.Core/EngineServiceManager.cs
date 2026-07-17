using Microsoft.Extensions.Logging;

using SharpEngine.Core.Handlers;
using SharpEngine.Telemetry;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpEngine.Core;

/// <summary>
///     Contains all the engine handlers and manages their lifecycle.
/// </summary>
public class EngineServiceManager
{
    public readonly List<EngineHandler> Handlers = [];
    private readonly ILogger<EngineServiceManager> _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="EngineServiceManager"/>.
    /// </summary>
    /// <param name="logger">The logger to use for logging messages.</param>
    public EngineServiceManager(ILogger<EngineServiceManager>? logger = null)
    {
        _logger = logger ?? LoggingExtensions.CreateLogger<EngineServiceManager>();
    }

    /// <summary>
    ///     Registers a new engine handler and starts its operation.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public void RegisterHandler(EngineHandler handler)
    {
        _logger.LogDebug("Registering handler: '{Handler}'.", handler.GetType().Name);

        Handlers.Add(handler);

        _logger.LogDebug("Handler '{Handler}' registered successfully.", handler.GetType().Name);
    }

    public void StartHandlers()
    {
        foreach (var engineHandler in Handlers)
            engineHandler.Start();
    }

    /// <summary>
    ///     Stops all active handlers asynchronously by calling their StopAsync method.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StopAllAsync()
    {
        var stopTasks = Handlers.Select(handler => handler.StopAsync());
        await Task.WhenAll(stopTasks);
    }
}