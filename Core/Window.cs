using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Core.Interfaces;
using System;

namespace Core;

// In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
// with several point lights
public class Window : GameWindow
{
    private readonly IGame _game;
    private readonly Renderer _renderer;

    public Window(IGame game, Scene scene, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = game;

        _renderer = new Renderer(_game, scene);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        _renderer.Initialize();

        _game.Camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

        CursorState = CursorState.Grabbed;

        _game.Initialize();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        _renderer.Render(_game.Camera);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (!IsFocused)
            return;

        var frameTime = (float)args.Time;
        if (_game.CoreSettings.PrintFrameRate)
            Console.WriteLine($"FPS: {1f / frameTime}");

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        _game.Camera.UpdateMousePosition(new Vector2(MouseState.X, MouseState.Y));

        _game.HandleKeyboard(KeyboardState, frameTime);
        _game.HandleMouse(MouseState);

        _game.Update(args, KeyboardState, MouseState);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _game.Camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        _game.Camera.AspectRatio = Size.X / (float)Size.Y;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        _game.HandleMouseDown(e);
    }
}
