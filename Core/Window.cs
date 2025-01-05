using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

using Core.Interfaces;
using Core.Renderers;
using Core.Enums;
using Core.Entities;

using System;
using Core.Shaders;
using System.Runtime;
using System.Collections.Generic;

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
        _uiRenderer = new UIRenderer(scene, _game);
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

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ToggleWireFrame(_game.CoreSettings.UseWireFrame);

        UseShaders();

        // TODO: Multi threading for different renderers
        if (_game.CoreSettings.RendererFlags.HasFlag(_renderer.RenderFlag))
            _renderer.Render();

        if (_game.CoreSettings.RendererFlags.HasFlag(_uiRenderer.RenderFlag))
            _uiRenderer.Render();

        SwapBuffers();
    }

    /// <summary>
    ///     Toggles the renderer between wireframe and fill mode.
    /// </summary>
    /// <param name="useWireFrame">Determines whether objects should be rendered in wireframe.</param>
    private static void ToggleWireFrame(bool useWireFrame)
        => GL.PolygonMode(MaterialFace.FrontAndBack, useWireFrame ? PolygonMode.Line : PolygonMode.Fill);

    private List<Shader> _shaders = [];

    private void UseShaders()
    {
        if (ShaderService.Instance.HasShadersToLoad)
            _shaders = ShaderService.Instance.GetAll();

        _shaders.ForEach(shader => shader.Use());
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
