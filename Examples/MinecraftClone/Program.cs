using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SharpEngine.Core.DependencyInjection;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Renderers.DependencyInjection;
using SharpEngine.Core.Scenes;

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

        services.AddRenderer<Renderer>();
        services.AddRenderer<UIRenderer>();

        services.AddEngine(engine =>
        {
            engine.AddHandler<WindowHandler>()
                  .AddWindow()
                  .AddWindow((sp) => {
                      // create second window with offset position
                      var game = sp.GetRequiredService<Game>();
                      var scene = sp.GetRequiredService<Scene>();
                      var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SharpEngine.Core.Windowing.Window>>();
                      var renderers = sp.GetServices<SharpEngine.Core.Renderers.RendererBase>();
                      var settings = new SharpEngine.Core.Entities.Views.Settings.DefaultViewSettings() with
                      {
                          WindowOptions = Silk.NET.Windowing.WindowOptions.Default with
                          {
                              Title = "Second Window",
                              Position = new Silk.NET.Maths.Vector2D<int>(700, 200)
                          }
                      };

                      return new SharpEngine.Core.Windowing.Window(game.Camera, scene, settings, logger, renderers);
                  }, isDefaultWindow: false);
        });
    }
}
