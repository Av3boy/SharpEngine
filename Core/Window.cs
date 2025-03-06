using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using MouseButton = Silk.NET.Input.MouseButton;

using Core.Renderers;
using Core.Entities;
using Core.Entities.Properties;
using Core.Shaders;
using SharpEngine.Core;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Entities.Views.Settings;
using Shader = Core.Shaders.Shader;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Interfaces;

namespace Core;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private readonly IGame _game;

    // TODO: Support multiple renderes (#19)
    private Renderer _renderer;
    private UIRenderer _uiRenderer;

    protected Scene Scene { get; private set; }
    public override string Title { get; set; }

    private readonly IWindow _window;

    public static GL GL;

    public IInputContext Input;
    private ImGuiController _imGuiController;

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="game">Contains the actual game implementation.</param>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="options"></param>
    public Window(IGame game, Scene scene, WindowOptions options)
    {
        // TODO: Game should be refactored out of the window class.
        _game = game;
        Scene = scene;

        _window = Silk.NET.Windowing.Window.Create(options);
        _game.Camera = new CameraView(Vector3.UnitZ * 3, new DefaultViewSettings());

        _window.Update += OnUpdateFrame;
        _window.Render += RenderFrame;
        _window.Resize += OnResize;
        _window.Load += OnLoad;

        _window.Run();
    }

    /// <inheritdoc />
    public override void OnLoad()
    {
        GL = _window.CreateOpenGL();

        Input = _window.CreateInput();
        _window.MakeCurrent();

        AssignInputEvents();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        // TODO: Set cursor shape
        // CursorShape = CursorShape.Hand;

        // Load all meshes from the mesh cache
        MeshService.Instance.LoadMesh("cube", SharpEngine.Core.Primitives.Cube.Mesh);
        _game.Initialize();

        _renderer = new Renderer(_game, Scene);
        _uiRenderer = new UIRenderer(Scene, _game);

        _renderer.Initialize();
        _uiRenderer.Initialize();

        _imGuiController = new ImGuiController(GL, _window, Input);
    }

    /// <summary>
    ///    Renders the current view and all specified renderers.
    /// </summary>
    /// <param name="deltaTime">The time since the last frame.</param>
    protected void RenderFrame(double deltaTime)
    {
        try
        {
            PreRender(deltaTime);

            _imGuiController.Update((float)deltaTime);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ToggleWireFrame(_game.CoreSettings.UseWireFrame);

            UseShaders();

            var renderTasks = new List<Task>();

            if (_game.CoreSettings.RendererFlags.HasFlag(_renderer.RenderFlag))
                renderTasks.Add(_renderer.Render());

            if (_game.CoreSettings.RendererFlags.HasFlag(_uiRenderer.RenderFlag))
                renderTasks.Add(_uiRenderer.Render());

            Task.WaitAll([.. renderTasks]);

            // TODO: This call causes filckering in the new framework. Investigate why.
            // _window.SwapBuffers();

            AfterRender(deltaTime);

            _imGuiController.Render();
        }
        catch (Exception ex)
        {
            Debug.LogInformation(ex.Message);
        }
    }

    protected virtual void PreRender(double deltaTime) { }

    protected virtual void AfterRender(double deltaTime) { }

    /// <summary>
    ///     Toggles the renderer between wireframe and fill mode.
    /// </summary>
    /// <param name="useWireFrame">Determines whether objects should be rendered in wireframe.</param>
    private static void ToggleWireFrame(bool useWireFrame)
        => GL.PolygonMode(GLEnum.FrontAndBack, useWireFrame ? PolygonMode.Line : PolygonMode.Fill);

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
        // TODO: GLFW_ISFOCUSED
        // if (!_window.IsFocused)
        //     return;

        if (_game.CoreSettings.PrintFrameRate)
            Console.WriteLine($"FPS: {1f / deltaTime}");

        // TODO: Handle multiple mice?
        var mouse = Input.Mice[0];
        var keyboard = Input.Keyboards[0];

        _game.Camera.UpdateMousePosition(mouse.Position);

        _game.HandleMouse(mouse);

        if (keyboard.IsKeyPressed(Key.Escape))
            _window.Close();

        _game.HandleKeyboard(keyboard, deltaTime);

        _game.Update(deltaTime, Input);
    }

    // TODO: #21 Input system
    private void AssignInputEvents()
    {
        foreach (var keyboard in Input.Keyboards)
          keyboard.KeyDown += KeyDown;

        foreach (var mouse in Input.Mice)
        {
            mouse.Scroll += OnMouseWheel;
            mouse.Click += OnMouseClick;
            mouse.MouseDown += OnMouseDown;
        }
    }

    /// <inheritdoc />
    protected void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            _window.Close();

        // _game.HandleKeyboard(keyboard);
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
    protected void OnMouseDown(IMouse mouse, MouseButton button)
        => _game.HandleMouseDown(mouse, button);

    /// <summary>
    ///     Sets the current scene.
    /// </summary>
    /// <param name="scene">The contents of the new scene.</param>
    protected void SetScene(Scene scene)
    {
        // TODO: Do we need to clear anything from e.g. the GPU when we change change the scene?
        Scene = scene;
    }

    public void Dispose()
    {
        // TODO: Dispose of any / all resources

        _imGuiController.Dispose();
    }
}
