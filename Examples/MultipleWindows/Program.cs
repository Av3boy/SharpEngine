using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Concurrent;
using SharpEngine.Core;
using SharpEngine.Core.Entities.Views.Settings;

// An example provided a lovely person in this thread:
// https://github.com/dotnet/Silk.NET/issues/2436#issuecomment-2752966073
public static partial class Program
{
    private static readonly List<IWindow> _windows = [];
    private static readonly List<IInputContext> _inputContexts = [];
    private static readonly ConcurrentQueue<WindowOptions> _windowQueue = [];
    private static readonly CancellationTokenSource _cancellationTokenSource = new();

    public static void Main(string[] _)
    {
        StartWindowQueue();

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            for (int i = 0; i < _windows.Count; i++)
                UpdateWindow(ref i);

            DequeueWindows();
        }
    }

    private static void UpdateWindow(ref int i)
    {
        var window = _windows[i];
        if (window is null)
            return;

        window.DoEvents();
        window.DoUpdate();
        window.DoRender();

        if (window.IsClosing)
        {
            window.Reset();
            window.Dispose();

            _windows.RemoveAt(i);
            i--; // Adjust the index to account for the removed item
            
            if (_windows.Count == 0)
                _cancellationTokenSource.Cancel();
        }
    }

    private static void DequeueWindows()
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            return;
        
        while (_windowQueue.TryDequeue(out var options))
            CreateWindow(options);
    }

    private static void StartWindowQueue()
        => Task.Run(async () =>
        {
            _windowQueue.Enqueue(WindowOptions.Default);
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(1000);
                Console.WriteLine("Running loop on background thread...");
            }
        });

    private static SharpEngine.Core.Window CreateWindow()
    {
        var options = new DefaultViewSettings() with
        {
            WindowOptions = WindowOptions.Default with
            {
                Title = "Window" + _windows.Count,

                // This is to make sure the windows don't overlap
                Position = new Vector2D<int>(
                    x: 500 + (50 * _windows.Count),
                    y: 400 + (50 * _windows.Count))
            }
        };

        //var window = Window.Create(options);
        var window = new SharpEngine.Core.Window(new(), options);
        window.Initialize();

        return window;
    }

    private static void CreateWindow(WindowOptions options)
    {
        var window = CreateWindow();
        foreach (var mouse in window.Input.Mice)
            mouse.Click += Mouse_Click;

        _inputContexts.Add(window.Input);
        _windows.Add(window);
    }

    private static void Mouse_Click(IMouse args1, Silk.NET.Input.MouseButton arg2, System.Numerics.Vector2 arg3)
    {
        _windowQueue.Enqueue(WindowOptions.Default);
    }
}
