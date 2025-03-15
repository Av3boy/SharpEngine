using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using MouseButton = Silk.NET.Input.MouseButton;

using SharpEngine.Core.Scenes;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Shaders;
using Shader = SharpEngine.Core.Shaders.Shader;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using SharpEngine.Core.Entities.Properties.Meshes;

namespace SharpEngine.Core;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private readonly Game _game;
    private static IWindow CurrentWindow;

    private IEnumerable<RendererBase> _renderers = [];

    private ImGuiController? _imGuiController;

    public IInputContext Input { get; protected set; }

    /// <summary>
    ///     The scene that is currently being rendered.
    /// </summary>
    protected Scene Scene { get; private set; }

    /// <summary>The OpenGL context.</summary>
    public static GL GL;

    // TODO: Use this method.
    /// <summary>
    ///     Gets the current OpenGL context.
    /// </summary>
    /// <returns>The OpenGL context for this window.</returns>
    public static GL GetGL() => GL;
    private static void SetGL(GL gl) => GL = gl;

    private bool _initialized;

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="game">Contains the actual game implementation.</param>
    /// <param name="scene">Contains the game scene.</param>
    public Window(Game game, Scene scene, WindowOptions options)
    {
        // TODO: Game should be refactored out of the window class.
        _game = game;
        Scene = scene;

        Initialize(options);
    }

    public Window(Scene scene, WindowOptions options)
    {
        _game = new Game();
        Scene = scene;

        Initialize(options);
    }

    private void Initialize(WindowOptions options)
    {
        CurrentWindow = CreateWindow(options);
        CurrentWindow.Update += OnUpdateFrame;
        CurrentWindow.Render += RenderFrame;
        CurrentWindow.Resize += OnResize;
        CurrentWindow.Load += OnLoad;
        CurrentWindow.Closing += OnClosing;
    }

    /// <inheritdoc />
    public override void Run(Action onFrame)
    {
        base.Run(onFrame);
        Run();
    }

    /// <inheritdoc />
    public override void OnLoad()
    {
        try
        {
            var context = CurrentWindow.CreateOpenGL();
            SetGL(context);

            Input = CurrentWindow.CreateInput();
            CurrentWindow.MakeCurrent();

            AssignInputEvents();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // TODO: Set cursor shape
            // CursorShape = CursorShape.Hand;

            // Load all meshes from the mesh cache
            MeshService.Instance.LoadMesh("cube", Primitives.Cube.Mesh);
            _game.Initialize();

            // Using reflection, find all renderers that implement the RendererBase.
            var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(RendererBase)) && !type.IsAbstract);

            foreach (var type in rendererTypes)
            {
                // Make sure the renderer has the correct constructor parameters!
                // TODO: The static reference to the context will not work when multiple windows are implemented, since the context will be different.
                // Also, the renderer should know nothing about the game, instead the camera should be passed.
                var requiredArguments = new object[] { _game, Scene };
                var renderer = (RendererBase)Activator.CreateInstance(type, requiredArguments)!;

                _renderers = _renderers.Append(renderer);
            }

            foreach (var renderer in _renderers)
                renderer.Initialize();

            _imGuiController = new ImGuiController(GL, CurrentWindow, Input);

            _initialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogInformation("Error loading window: " + ex.Message, ex);
        }

        base.OnLoad();
    }

    /// <summary>
    ///    Renders the current view and all specified renderers.
    /// </summary>
    /// <param name="deltaTime">The time since the last frame.</param>
    protected void RenderFrame(double deltaTime)
    {
        while (!_initialized)
        {
            // Wait for the window to be initialized.
        }

        try
        {
            PreRender(deltaTime);

            _imGuiController?.Update((float)deltaTime);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ToggleWireFrame(_game.CoreSettings.UseWireFrame);

            UseShaders();

            var renderTasks = _renderers.Where(renderer => _game.CoreSettings.RendererFlags.HasFlag(renderer.RenderFlag))
                                        .Select(renderer => renderer.Render())
                                        .ToList();

            Task.WaitAll([.. renderTasks]);

            // TODO: This call causes filckering in the new framework. Investigate why.
            // _window.SwapBuffers();

            AfterRender(deltaTime);

            _imGuiController?.Render();
        }
        catch (Exception ex)
        {
            Debug.LogInformation(ex.Message);
        }
    }

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
        while (!_initialized)
        {
            // Wait for the window to be initialized.
        }

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
            CurrentWindow.Close();

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

    public virtual void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector) { }


    /// <inheritdoc />
    protected void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            CurrentWindow.Close();

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
        _game.Camera.AspectRatio = size.X / size.Y;
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

    /// <inheritdoc />
    protected void Dispose(bool disposing)
    {
        //base.Dispose(disposing);
    }
}
