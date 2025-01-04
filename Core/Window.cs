using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

using Core.Interfaces;
using System;
using Core.Renderers;

namespace Core;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : GameWindow
{
    private readonly IGame _game;
    private readonly Renderer _renderer;
    private readonly UIRenderer _uiRenderer;

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="game">Contains the actual game implementation.</param>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="gameWindowSettings"><inheritdoc cref="GameWindowSettings"/></param>
    /// <param name="nativeWindowSettings"><inheritdoc cref="NativeWindowSettings"/></param>
    public Window(IGame game, Scene scene, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = game;
        _game.Camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

        _renderer = new Renderer(_game, scene);
        _uiRenderer = new UIRenderer(scene, _game.Camera);
    }

    /// <inheritdoc />
    protected override void OnLoad()
    {
        base.OnLoad();

        _renderer.Initialize();
        _uiRenderer.Initialize();

        CursorState = CursorState.Grabbed;

        _game.Initialize();
    }

    /// <inheritdoc />
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // TODO: Multi threading for different renderers
        _renderer.Render(_game.Camera);
        _uiRenderer.Render();

        SwapBuffers();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        var direction = e.OffsetY switch
        {
            > 0 => MouseWheelScrollDirection.Up,
            < 0 => MouseWheelScrollDirection.Down,
            _ => throw new NotImplementedException()
        };

        _game.HandleMouseWheel(direction, e);

        _game.Camera.Fov -= e.OffsetY;
    }

    /// <inheritdoc />
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        _game.Camera.AspectRatio = Size.X / (float)Size.Y;
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        _game.HandleMouseDown(e);
    }
}
