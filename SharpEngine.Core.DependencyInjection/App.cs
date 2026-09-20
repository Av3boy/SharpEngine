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
    private readonly IConfiguration? _configuration;
    private readonly IServiceProvider _serviceProvider;
    private Engine _engine;

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

        var game = _serviceProvider.GetService<Game>();
        if (game is null)
            throw new InvalidOperationException("A service for the type 'Game' could not be found. One must be registered before running the application with the dependency Injection apporach.");

        _engine = _serviceProvider.GetRequiredService<Engine>();
        if (_engine is null)
            throw new InvalidOperationException("A service for the type 'Engine' could not be found. One must be registered before running the application with the dependency Injection apporach.");

        try
        {
            logger.LogInformation("App started. Resolving services.");

            if (cancellationToken.IsCancellationRequested)
                return;

            logger.LogInformation("Starting engine handlers.");
            _engine.Initialize();
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
