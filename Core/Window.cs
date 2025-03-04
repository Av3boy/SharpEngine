using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

using Core.Interfaces;
using Core.Renderers;
using Core.Entities;
using Core.Shaders;

using SharpEngine.Core;
using SharpEngine.Core.Scenes;
using Shader = Core.Shaders.Shader;
using System.Numerics;

namespace Core;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    // TODO: Support multiple renderes (#19)
    private Renderer _renderer;
    private UIRenderer _uiRenderer;
    private ImGuiController _imGuiController;

    protected ISettings Settings;
    protected Scene Scene { get; private set; }
    protected readonly View View;

    private readonly IWindow _window;

    public static GL GL;
    public IInputContext Input;

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings"></param>
    /// <param name="view"></param>
    public Window(Scene scene, ISettings settings, View view)
    {
        // TODO: Game should be refactored out of the window class.
        Scene = scene;
        Settings = settings;
        View = view;

        _window = Silk.NET.Windowing.Window.Create(Settings.WindowOptions);
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

        _renderer = new Renderer(View, Scene, Settings);
        _uiRenderer = new UIRenderer(View, Scene, Settings);

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

            ToggleWireFrame(Settings.UseWireFrame);

            UseShaders();

            var renderTasks = new List<Task>();

            if (Settings.RendererFlags.HasFlag(_renderer.RenderFlag))
                renderTasks.Add(_renderer.Render());

            if (Settings.RendererFlags.HasFlag(_uiRenderer.RenderFlag))
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

        if (Settings.PrintFrameRate)
            Console.WriteLine($"FPS: {1f / deltaTime}");

        // TODO: Handle multiple mice?
        var mouse = Input.Mice[0];
        var keyboard = Input.Keyboards[0];

        View.UpdateMousePosition(mouse.Position);

        if (keyboard.IsKeyPressed(Key.Escape))
            _window.Close();
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
    }

    /// <inheritdoc />
    protected void OnMouseWheel(IMouse mouse, ScrollWheel sw)
    {
        // _camera.Fov -= sw.Y;
    }

    /// <inheritdoc />
    protected void OnResize(Vector2D<int> size)
    {
        GL.Viewport(size);

        if (_window.WindowState != WindowState.Minimized)
            View.AspectRatio = size.X / (float)size.Y;
    }

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
