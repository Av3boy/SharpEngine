using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SharpEngine.Core.Windowing;

using System;
using System.Threading;

namespace SharpEngine.Core.DependencyInjection;

/// <summary>
///     Represents the entry point of the application , responsible for resolving required services and running the main loop or window.
/// </summary>
public class App
{
    private static IConfiguration? _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    /// <param name="configuration">The application configuration.</param>
    public App(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
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
            if (windowFactory is not null && windowFactory.RegisteredWindows.Count > 0)
            {
                using var factoryWindow = windowFactory.CreateWindow();

                logger.LogInformation("Required services resolved.");
                logger.LogInformation("Starting window.");

                factoryWindow.Run();
                return;
            }

            var window = _serviceProvider.GetService<Window>();
            if (window is not null)
            {
                logger.LogInformation("Required services resolved.");
                logger.LogInformation("Starting window.");

                using (window)
                    window.Run();

                return;
            }

            // This is just an example of how to use the scopes. In App we should always use '_serviceProvider'.
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            logger.LogInformation("Required services resolved.");
            logger.LogInformation("Entering main loop.");

            bool running = !cancellationToken.IsCancellationRequested;
            while (running)
            {

                //system.Update();
                logger.LogInformation("frame tick.");

                Thread.Sleep(1000); // Simulate frame delay
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred.");
        }
    }

    /// <summary>
    ///     Runs the application by resolving required services and starting either a window or a main processing loop.
    /// </summary>
    public void Run() => Run(CancellationToken.None);
}
