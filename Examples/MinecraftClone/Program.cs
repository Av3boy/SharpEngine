using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SharpEngine.Core.DependencyInjection;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;

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
        services.AddSingleton<CameraView>(serviceProvider => serviceProvider.GetRequiredService<Minecraft>().Camera);
        services.AddTransient<RendererBase, Renderer>();
        services.AddTransient<RendererBase, UIRenderer>();

        services.AddWindow(
            factory: serviceProvider =>
            {
                var game = serviceProvider.GetRequiredService<Minecraft>();
                var scene = serviceProvider.GetRequiredService<Scene>();
                var windowLogger = serviceProvider.GetRequiredService<ILogger<Window>>();
                var renderers = serviceProvider.GetServices<RendererBase>();

                return new Window(game.Camera, scene, game.Camera.Settings, windowLogger, renderers);
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

                Minecraft.Window = window;
            },
            name: "minecraft",
            isDefault: true);
    }
}
