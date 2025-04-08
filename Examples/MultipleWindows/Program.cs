using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

internal class Program
{
    static void Main()
    {
        var options1 = WindowOptions.Default with { Title = "Window 1", WindowClass = "MyWindowClass1" };
        var options2 = WindowOptions.Default with { Title = "Window 2", WindowClass = "MyWindowClass2" };

        var window1 = Window.Create(options1);
        var window2 = Window.Create(options2);

        StartWindow(window1);
        StartWindow(window2);
    }

    static void StartWindow(IWindow window)
    {
        window.Load += () =>
        {
            Console.WriteLine($"Hello from window: " + window.Title);

            var gl = GL.GetApi(window);

            unsafe
            {
                var versionPtr = gl.GetString(StringName.Version);
                string? version = SilkMarshal.PtrToString((nint)versionPtr);
                Console.WriteLine($"OpenGL version: {version}");
            }

        };

        window.Run();
    }
}
