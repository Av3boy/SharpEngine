using SharpEngine.Core.Interfaces;
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
        DefaultSettings gameSettings = new()
        {
            UseWireFrame = false
        };

        Scene scene = new Scene();
        Minecraft game = new Minecraft(scene, gameSettings);

        using var window = new SharpEngine.Core.Windowing.Window(game.Camera, scene, game.Camera.Settings);
        window.OnLoaded += game.Initialize;
        window.OnHandleMouse += game.HandleMouse;
        window.OnUpdate += game.Update;
        window.OnHandleKeyboard += game.HandleKeyboard;
        window.OnButtonMouseDown += game.HandleMouseDown;
        window.HandleMouseWheel += game.HandleMouseWheel;
        window.OnAfterRender += deltaTime => game.OnAfterRender(new Frame(deltaTime));

        // TODO: This needs to be streamlined. 
        game.Window = window;

        window.Run();
    }
}
