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
        services.AddSingleton<Game, Minecraft>();
        services.AddSingleton<CameraView>(serviceProvider => serviceProvider.GetRequiredService<Game>().Camera);
        services.AddTransient<RendererBase, Renderer>();
        services.AddTransient<RendererBase, UIRenderer>();

        services.AddWindow(
            configure: (serviceProvider, window) =>
            {
                var game = serviceProvider.GetRequiredService<Game>();

                window.OnLoaded += game.Initialize;
                window._inputManager.OnHandleMouse += game.HandleMouse;
                window._inputManager.OnUpdate += game.Update;
                window._inputManager.OnHandleKeyboard += game.HandleKeyboard;
                window._inputManager.OnButtonMouseDown += game.HandleMouseDown;
                window._inputManager.HandleMouseWheel += game.HandleMouseWheel;
                window.OnAfterRender += game.OnAfterRender;

                Minecraft.Window = window;
            },
            name: "minecraft",
            isDefaultWindow: true);
    }
}
