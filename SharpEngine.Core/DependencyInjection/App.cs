using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpEngine.Core.Windowing;
using System;
using System.Threading;

namespace DITesting;

public class App
{
    private static IConfiguration? _configuration;
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    // TODO: Integrate into the existing windowing / startup system.
    public void Run(CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("app started. Resolving services.");

            if (cancellationToken.IsCancellationRequested)
                return;

            var windowFactory = _serviceProvider.GetService<IWindowFactory>();
            if (windowFactory is not null && windowFactory.RegisteredWindows.Count > 0)
            {
                using var factoryWindow = windowFactory.CreateWindow();

                Console.WriteLine("Required services resolved.");
                Console.WriteLine("Starting window.");

                factoryWindow.Run();
                return;
            }

            var window = _serviceProvider.GetService<Window>();
            if (window is not null)
            {
                Console.WriteLine("Required services resolved.");
                Console.WriteLine("Starting window.");

                using (window)
                    window.Run();

                return;
            }

            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            // This is just an example of how to use the scopes. In App we should always use '_serviceProvider'.
            //var system = scope.ServiceProvider.GetRequiredService<FrameSystemInterface>();

            Console.WriteLine("Required services resolved.");
            Console.WriteLine("Entering main loop.");

            bool running = !cancellationToken.IsCancellationRequested;
            while (running)
            {

                //system.Update();
                Console.WriteLine("frame tick.");

                Thread.Sleep(1000); // Simulate frame delay
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex}");
        }
    }

    public void Run() => Run(CancellationToken.None);
}
