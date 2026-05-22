using SharpEngine.Shared;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpEngine.Core;

/// <summary>
///     Contains all the engine handlers and manages their lifecycle.
/// </summary>
public class EngineServiceManager
{
    private readonly List<EngineHandler> handlers = [];

    /// <summary>
    ///     Registers a new engine handler and starts its operation.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public void RegisterHandler(EngineHandler handler)
    {
        Debug.Log.Debug("Registering handler: '{Handler}'", handler.GetType().Name);

        handlers.Add(handler);
        handler.Start();

        Debug.Log.Debug("Handler '{Handler}' registered successfully.", handler.GetType().Name);
    }

    /// <summary>
    ///     Stops all active handlers asynchronously by calling their StopAsync method.
    /// </summary>
    /// <returns>This method does not return a value.</returns>
    public async Task StopAllAsync()
    {
        var stopTasks = handlers.Select(handler => handler.StopAsync());
        await Task.WhenAll(stopTasks);
    }
}