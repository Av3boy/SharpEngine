using Core;
using ImGuiNET;
using SharpEngine.Core.Scenes;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Minecraft;

/// <summary>
///     Represents the entry point of the application.
/// </summary>
public static class Program
{
    private static void Main()
    {
        Settings gameSettings = new()
        {
            UseWireFrame = false,
            WindowOptions = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Minecraft",
            }
        };

        Scene scene = new Scene();
        Game game = new Game(scene, gameSettings);

        using var window = new Core.Window(scene, gameSettings, game.Camera);
        window.Load += game.Initialize;

        window.Run();

        // game.Initialize();
    }
}
