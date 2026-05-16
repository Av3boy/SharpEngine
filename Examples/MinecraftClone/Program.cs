using DITesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        services.AddSingleton<Minecraft>();
        services.AddWindow(
            factory: serviceProvider =>
            {
                var game = serviceProvider.GetRequiredService<Minecraft>();
                var scene = serviceProvider.GetRequiredService<Scene>();
                var settings = serviceProvider.GetRequiredService<ISettings>();
                var windowLogger = serviceProvider.GetRequiredService<ILogger<Window>>();
                var rendererLogger = serviceProvider.GetRequiredService<ILogger<Renderer>>();
                var uiRendererLogger = serviceProvider.GetRequiredService<ILogger<UIRenderer>>();

                var rendererFactories = new Func<Window, RendererBase>[]
                {
                    window => new Renderer(game.Camera, window, settings, scene, rendererLogger),
                    window => new UIRenderer(game.Camera, window, settings, scene, uiRendererLogger)
                };

                return new Window(game.Camera, scene, game.Camera.Settings, rendererFactories, windowLogger);
            },
            configure: (serviceProvider, window) =>
            {
                var game = serviceProvider.GetRequiredService<Minecraft>();

                window.OnLoaded += game.Initialize;
                window.OnHandleMouse += game.HandleMouse;
                window.OnUpdate += game.Update;
                window.OnHandleKeyboard += game.HandleKeyboard;
                window.OnButtonMouseDown += game.HandleMouseDown;
                window.HandleMouseWheel += game.HandleMouseWheel;
                window.OnAfterRender += game.OnAfterRender;

                game.Window = window;
            },
            name: "minecraft",
            isDefault: true);
    }
}
