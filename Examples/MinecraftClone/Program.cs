using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpEngine.Core;
using SharpEngine.Core.DependencyInjection;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;
using System;

namespace Minecraft;

/// <summary>
///     Represents the entry point of the application.
/// </summary>
public static class Program
{
    private static void Main()
    {
        var builder = new AppBuilder()
            .ConfigureServices(ConfigureServices);

        var app = builder.Build();
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddConsole());

        services.AddSingleton<ISettings>(_ => new DefaultSettings
        {
            UseWireFrame = false
        });

        services.AddSingleton<Scene>();
        services.AddSingleton<Game, Minecraft>();
        services.AddSingleton<CameraView>(serviceProvider => serviceProvider.GetRequiredService<Game>().Camera);
        services.AddTransient<RendererBase, Renderer>();
        services.AddTransient<RendererBase, UIRenderer>();

        services.AddEngine(engine =>
        {
            engine.AddHandler<WindowHandler>().AddWindow();
        });
    }
}
