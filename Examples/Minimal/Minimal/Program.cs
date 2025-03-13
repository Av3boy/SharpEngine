using SharpEngine.Core;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SharpEngine.Core
{
    // TODO: Minimal isn't really "minimal"
    // TODO: Make this project work.

    public static class Program
    {
        public static void Main(string[] args)
        {
            var game = new Game();
            var window = new Window(game, new Scene(), new WindowOptions() with
            {
                Title = "SharpEngine minimal game",
                Size = new Vector2D<int>(800, 600)
            });

            window.Run();
        }
    }

    public class Settings : ISettings
    {
        public WindowOptions WindowOptions { get; set; } = new WindowOptions();
        public bool UseWireFrame { get; set; }
        public bool PrintFrameRate { get; set; }
        public RenderFlags RendererFlags { get; set; }
    }

    public class Game : IGame
    {
        public ISettings CoreSettings => new Settings();

        public CameraView Camera { get; set; }

        public void HandleKeyboard(IKeyboard input, double deltaTime) { }
        public void HandleMouse(IMouse mouse) { }
        public void HandleMouseDown(IMouse mouse, MouseButton button) { }
        public void HandleMouseWheel(MouseWheelScrollDirection direction, ScrollWheel scrollWheel) { }
        public void Initialize() { }
        public void Update(double deltaTime, IInputContext input) { }
    }
}
