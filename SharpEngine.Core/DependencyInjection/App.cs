using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

    public void Run(CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("app started. Resolving services.");

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
