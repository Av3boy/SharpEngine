using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DITesting;

internal class Program
{
    static void Main(string[] _)
    {
        // Build configuration
        // var configuration = new ConfigurationBuilder()
        //     .AddJsonFile("appsettings.json", optional: true)
        //     .AddEnvironmentVariables()
        //     .Build();

        var builder = new AppBuilder()
                        .Configure(cfg =>
                        {
                            // cfg.AddJsonFile("appsettings.json", optional: true)
                            //    .AddEnvironmentVariables();
                        })
                        .ConfigureServices(services =>
                        {
                            services.AddTransient<FrameSystemInterface, FrameSystem>();
                        });

        var app = builder.Build();

        using var cts = new CancellationTokenSource();
        app.Run(cts.Token);
    }

    static void Main_(string[] args)
    {
        new AppBuilder().Build().Run();
    }
}

internal class AppBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private IConfiguration? _configuration;

    private App App;

    public AppBuilder Configure(Action<IConfigurationBuilder> configure)
    {
        var configBuilder = new ConfigurationBuilder();
        configure(configBuilder);

        _configuration = configBuilder.Build();
        _services.AddSingleton<IConfiguration>(_configuration);

        return this;
    }

    public AppBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    public App Build()
    {
        if (_configuration == null)
            throw new InvalidOperationException("Configuration must be set before building the app.");

        App = new App(_services.BuildServiceProvider(), _configuration);
        return App;
    }
}

internal class App
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
            var system = scope.ServiceProvider.GetRequiredService<FrameSystemInterface>();

            Console.WriteLine("Required services resolved.");
            Console.WriteLine("Entering main loop.");

            bool running = !cancellationToken.IsCancellationRequested;
            while (running)
            {

                system.Update();

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

internal interface FrameSystemInterface
{
    void Update();
}

internal class FrameSystem : FrameSystemInterface
{
    public void Update()
    {
        Console.WriteLine("Frame Updated.");
        // Example usage:
        // var setting = App.Configuration["SomeSetting"];
    }
}
