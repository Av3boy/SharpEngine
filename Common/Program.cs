using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LearnOpenTK
{
    public static class Program
    {
        private static void Main()
        {
            Scene scene = new Scene();
            Game game = new Game(scene);

            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 600),
                Title = "LearnOpenTK - Multiple lights",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            using var window = new Window(game, scene, GameWindowSettings.Default, nativeWindowSettings);
            window.Run();
        }
    }
}
