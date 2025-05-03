using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using MouseButton = Silk.NET.Input.MouseButton;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Shader = SharpEngine.Core.Shaders.Shader;
using Silk.NET.GLFW;
using SharpEngine.Shared;

namespace SharpEngine.Core.Windowing;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private bool _initialized;
    private IEnumerable<RendererBase> _renderers = [];
    private ImGuiController? _imGuiController;

    /// <summary>
    ///     Gets or sets the view for the window.
    /// </summary>
    public readonly CameraView Camera;

    /// <summary>
    ///     Gets the settings for the current window.
    /// </summary>
    public IViewSettings Settings;

    /// <summary>The event executed when mouse events are executed.</summary>
    public event Action<IMouse>? OnHandleMouse;

    /// <summary>The event executed when keyboard events are executed.</summary>
    public event Action<IKeyboard, double>? OnHandleKeyboard;

    /// <summary>The event executed when the window is updated.</summary>
    public event Action<double, IInputContext>? OnUpdate;

    /// <summary>The event executed when the mouse wheel is scrolled.</summary>
    public event Action<MouseWheelScrollDirection, ScrollWheel>? HandleMouseWheel;

    /// <summary>The event executed when a mouse button is clicked.</summary>
    public event Action<IMouse, MouseButton>? OnButtonMouseDown;

    /// <summary>
    ///     The scene that is currently being rendered.
    /// </summary>
    protected Scene Scene { get; private set; }

    /// <summary>The OpenGL context.</summary>
    public static GL GL;

    // TODO: #93 Use this method.
    /// <summary>
    ///     Gets the current OpenGL context.
    /// </summary>
    /// <returns>The OpenGL context for this window.</returns>
    public static GL GetGL() => GL;
    private static void SetGL(GL gl) => GL = gl;
    
    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="camera">The camera the window should render from.</param>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(CameraView camera, Scene scene, IViewSettings settings)
    {
        Scene = scene;
        Settings = settings;
        Camera = camera;
        InitializeWindow();
    }

    /// <summary>
    ///     Initializes a new window without a dedicated camera.
    /// </summary>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(Scene scene, IViewSettings settings)
    {
        Scene = scene;
        Settings = settings;
        Camera = new(Vector3.One, settings);

        InitializeWindow();
    }

    private bool _windowInitialized;

    private void InitializeWindow()
    {
        CurrentWindow = CreateWindow(Settings.WindowOptions);
        CurrentWindow.Update += deltaTime => OnUpdateFrame(new Frame(deltaTime));
        CurrentWindow.Render += deltaTime => RenderFrame(new Frame(deltaTime));
        CurrentWindow.Resize += OnResize;
        CurrentWindow.Load += OnLoad;
        CurrentWindow.Closing += OnClosing;
    }

    /// <inheritdoc />
    public override void Run(Action onFrame)
    {
        if (!_windowInitialized)
            throw new InvalidOperationException("Window not been initialized for this instance. Try calling 'window.Initialize()' first.");

        try
        {
            base.Run(onFrame);
            Run();
        }
        catch (Exception ex)
        {
            Debug.Log.Error(ex, "Error running window: {Message}", ex.Message);
        }
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

            SetWindowIcon(PathExtensions.GetAssemblyPath("_Resources/icon.png"));

            AssignInputEvents();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Load all meshes from the mesh cache
            // MeshService.Instance.LoadMesh("cube", Primitives.Cube.Mesh);

            // Using reflection, find all renderers that implement the RendererBase.
            var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(RendererBase)) && !type.IsAbstract);

            foreach (var type in rendererTypes)
            {
                // Make sure the renderer has the correct constructor parameters!
                // TODO: #75 The static reference to the context will not work when multiple windows are implemented, since the context will be different.
                var requiredArguments = new object[] { Camera, this, Settings, Scene };
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
            Debug.Log.Information(ex, "Error loading window: {Message}", ex.Message);
        }

        base.OnLoad();
    }

    /// <summary>
    ///    Renders the current view and all specified renderers.
    /// </summary>
    /// <param name="frame">Contains information about the previous frame.</param>
    protected void RenderFrame(Frame frame)
    {
        while (!_initialized)
        {
            // Wait for the window to be initialized.
        }

        try
        {
            PreRender(frame);

            _imGuiController?.Update((float)frame.FrameTime);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ToggleWireFrame(Settings.UseWireFrame);

            UseShaders();

            var renderTasks = _renderers.Where(renderer => Settings.RendererFlags.HasFlag(renderer.RenderFlag))
                                        .Select(renderer => renderer.Render())
                                        .ToList();

            Task.WaitAll([.. renderTasks]);

            AfterRender(frame);

            _imGuiController?.Render();
        }
        catch (Exception ex)
        {
            Debug.Log.Information(ex.Message);
        }
    }

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
    protected void OnUpdateFrame(Frame frame)
    {
        while (!_initialized)
        {
            // Wait for the window to be initialized.
        }

        if (Settings.PrintFrameRate)
            Debug.Log.Information($"FPS: {frame.FrameRate}");

        // TODO: #21 Handle multiple mice?
        var mouse = Input?.Mice[0];
        if (mouse is not null)
        {
            Camera.UpdateMousePosition(mouse.Position);
            OnHandleMouse?.Invoke(mouse);
        }

        var keyboard = Input?.Keyboards[0];
        if (keyboard is not null)
        {
            if (keyboard.IsKeyPressed(Key.Escape))
                CurrentWindow.Close();

            OnHandleKeyboard?.Invoke(keyboard, frame.FrameTime);
        }

        if (Input is not null)
            OnUpdate?.Invoke(frame.FrameTime, Input);
    }

    // TODO: #21 Input system
    private void AssignInputEvents()
    {
        if (Input is null)
        {
            Debug.Log.Information("Input is null. No input events will be assigned.");
            return;
        }

        foreach (var keyboard in Input.Keyboards)
          keyboard.KeyDown += KeyDown;

        foreach (var mouse in Input.Mice)
        {
            mouse.Scroll += OnMouseWheel;
            mouse.Click += OnMouseClick;
            mouse.MouseDown += OnMouseDown;

            if (IsFocused)
                mouse.Cursor.CursorMode = CursorMode.Raw;
        }
    }

    /// <inheritdoc />
    protected void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            CurrentWindow.Close();
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

        HandleMouseWheel?.Invoke(direction, sw);
        Camera.Fov -= sw.Y;
    }

    /// <inheritdoc />
    protected void OnResize(Vector2D<int> size)
    {
        GL.Viewport(size);
        
        if (size != Vector2D<int>.Zero)
            Camera.AspectRatio = (float)size.X / size.Y;
    }

    /// <inheritdoc />
    protected override void OnMouseDown(IMouse mouse, MouseButton button)
        => OnButtonMouseDown?.Invoke(mouse, button);

    /// <summary>
    ///     Sets the current scene.
    /// </summary>
    /// <param name="scene">The contents of the new scene.</param>
    protected void SetScene(Scene scene)
    {
        // TODO: #92 Do we need to clear anything from e.g. the GPU when we change change the scene?
        Scene = scene;
    }
}
