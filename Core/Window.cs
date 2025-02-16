using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Shader = Core.Shaders.Shader;

using Core.Interfaces;
using Core.Renderers;
using Core.Enums;
using Core.Entities;
using Core.Entities.Properties;
using Core.Shaders;

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Core;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private readonly IGame _game;
    private readonly Renderer _renderer;
    private readonly UIRenderer _uiRenderer;

    private static IWindow _window;
    private readonly WindowOptions _options;

    public static GL GL;

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="game">Contains the actual game implementation.</param>
    /// <param name="scene">Contains the game scene.</param>
    public Window(IGame game, Scene scene, WindowOptions options)
    {
        _window = Silk.NET.Windowing.Window.Create(options);
        GL = _window.CreateOpenGL();

        _options = options;
        _game = game;
        _game.Camera = new Camera(Vector3.UnitZ * 3, _window.Size.X / (float)_window.Size.Y);

        _window.Load += OnLoad;
        _window.Update += OnUpdateFrame;
        _window.Render += RenderFrame;

        _window.Resize += OnResize;

        var input = _window.CreateInput();
        AssignInputEvents(input);

        _window.Run();

        _renderer = new Renderer(_game, scene);
        _uiRenderer = new UIRenderer(scene, _game);
    }

    public void Run()
        => _window.Run();

    /// <inheritdoc />
    protected void OnLoad()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        // TODO: Set cursor shape
        // CursorShape = CursorShape.Hand;

        // Load all meshes from the mesh cache
        MeshService.Instance.LoadMesh("cube", Primitives.Cube.Mesh);

        _game.Initialize();

        _renderer.Initialize();
        _uiRenderer.Initialize();
    }

    /// <summary>
    ///    Renders the current view and all specified renderers.
    /// </summary>
    /// <param name="deltaTime"></param>
    protected void RenderFrame(double deltaTime)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ToggleWireFrame(_game.CoreSettings.UseWireFrame);

        UseShaders();

        // TODO: Multi threading for different renderers
        if (_game.CoreSettings.RendererFlags.HasFlag(_renderer.RenderFlag))
            _renderer.Render();

        if (_game.CoreSettings.RendererFlags.HasFlag(_uiRenderer.RenderFlag))
            _uiRenderer.Render();

        _window.SwapBuffers();
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
    protected void OnUpdateFrame(double deltaTime)
    {
        // if (!_window.IsFocused)
        //     return;

        if (_game.CoreSettings.PrintFrameRate)
            Console.WriteLine($"FPS: {1f / deltaTime}");

        _game.Camera.UpdateMousePosition(new Vector2(MouseState.X, MouseState.Y));

        _game.HandleMouse(MouseState);
        _game.Update(args, KeyboardState, MouseState);
    }

    private void AssignInputEvents(IInputContext input)
    {
        foreach (var keyboard in input.Keyboards)
            keyboard.KeyDown += KeyDown;

        foreach (var mouse in input.Mice)
        {
            mouse.Scroll += OnMouseWheel;
            mouse.Click += OnMouseClick;
            mouse.MouseDown += OnMouseDown;
        }
    }

    private void OnMouseClick(IMouse mouse, Silk.NET.Input.MouseButton button, Vector2 vector) => throw new NotImplementedException();

    protected void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            _window.Close();

        _game.HandleKeyboard(KeyboardState, frameTime);
    }

    /// <inheritdoc />
    protected void OnMouseWheel(IMouse mouse, ScrollWheel sw)
    {
        
        var direction = sw.Y switch
        {
            > 0 => MouseWheelScrollDirection.Up,
            < 0 => MouseWheelScrollDirection.Down,
            _ => throw new NotImplementedException()
        };

        _game.HandleMouseWheel(direction, sw);

        _game.Camera.Fov -= sw.Y;
    }

    /// <inheritdoc />
    protected void OnResize(Vector2D<int> size)
    {
        GL.Viewport(size);
        _game.Camera.AspectRatio = (float)(size.X / size.Y);
    }

    /// <inheritdoc />
    protected void OnMouseDown(IMouse mouse, Silk.NET.Input.MouseButton button)
    {
        _game.HandleMouseDown(mouse, button);
    }

    public void Dispose()
    {
        // TODO: Dispose of any / all resources
    }
}
