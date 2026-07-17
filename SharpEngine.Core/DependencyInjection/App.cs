using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SharpEngine.Core.Windowing;

using System;
using System.Linq;
using System.Threading;
using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;

namespace SharpEngine.Core.DependencyInjection;

/// <summary>
///     Represents the application entry point responsible for resolving required services and running the main loop or window.
/// </summary>
public class App
{
    private static IConfiguration? _configuration;
    private readonly IServiceProvider _serviceProvider;

    private readonly Engine _engine;

    /// <summary>
    ///     Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    /// <param name="configuration">The application configuration.</param>
    public App(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;

        _engine = new Engine();
    }

    /// <summary>
    ///     Runs the application by resolving required services and starting either a window or a main processing loop.
    /// </summary>
    /// <param name="cancellationToken">Signals that the operation should be canceled.</param>
    public void Run(CancellationToken cancellationToken)
    {
        var logger = _serviceProvider.GetService<ILogger<App>>()!;

        try
        {

            logger.LogInformation("App started. Resolving services.");

            if (cancellationToken.IsCancellationRequested)
                return;

            var windowFactory = _serviceProvider.GetService<IWindowFactory>();
            if (windowFactory is null || windowFactory.RegisteredWindows.Count <= 0)
            {
                logger.LogInformation("No registered windows found. Engine shutting down.");
                return;
            }

            var game = _serviceProvider.GetRequiredService<Game>();

            _engine.Initialize(game);
            var windowHandler = _engine.ServicesManager.Handlers;
            var a = windowHandler.First(h => h.GetType() == typeof(WindowHandler));

            // foreach (var window in windowFactory.CreateAllWindows())
            // {
            //     logger.LogInformation("Starting window '{windowName}'.", window.Title);
            // 
            //     // TODO: We might need to use the window handler instead to keep the execution of the window on a separate thread.
            //     window.Initialize();
            //     window.Run();
            // }

            logger.LogInformation("Starting engine handlers.");

            foreach (var handler in _engine.ServicesManager.Handlers)
            {
                handler.Start();
            }

            while (true)
            {
                
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred.");
        }
    }

    /// <summary>
    ///     Runs the application without a <see cref="CancellationToken"/>.
    /// </summary>
    public void Run() => Run(CancellationToken.None);
}
