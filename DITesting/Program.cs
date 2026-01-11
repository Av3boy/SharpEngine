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
