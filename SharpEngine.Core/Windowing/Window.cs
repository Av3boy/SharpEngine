using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using Microsoft.Extensions.Logging;

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

namespace SharpEngine.Core.Windowing;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private const string _iconPath = "_Resources/icon.png";
    private bool _windowInitialized;
    private bool _initialized;
    private readonly IReadOnlyList<Func<Window, RendererBase>> _rendererFactories;
    private readonly ILogger<Window> _logger;
    
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
    private GL _gl;

    private static GL? _sharedGl;

    /// <summary>
    ///     Gets the shared OpenGL instance used for resource creation when windows share an OpenGL context.
    /// </summary>
    /// <remarks>
    ///     This is primarily for backward compatibility with code that assumed a single global GL instance.
    ///     For multi-window support, prefer using <see cref="GetGL"/> and ensuring the correct context is current.
    /// </remarks>
    public static GL SharedGL
        => _sharedGl ?? throw new InvalidOperationException("No OpenGL context has been created yet.");

    /// <summary>
    ///     Backward compatible alias for <see cref="SharedGL"/>.
    /// </summary>
    public static GL GL => SharedGL;

    // TODO: #93 Use this method.
    /// <summary>
    ///     Gets the current OpenGL context.
    /// </summary>
    /// <returns>The OpenGL context for this window.</returns>
    public GL GetGL() => _gl;
    private void SetGL(GL gl) => _gl = gl;
    
    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="camera">The camera the window should render from.</param>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(CameraView camera, Scene scene, IViewSettings settings, IEnumerable<Func<Window, RendererBase>>? rendererFactories = null, ILogger<Window>? logger = null)
    {
        Scene = scene;
        Settings = settings;
        Camera = camera;
        _rendererFactories = rendererFactories?.ToArray() ?? [];
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Window>();

        InitializeWindow();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <remarks>
    ///     The new window is initialized without a dedicated camera.
    /// </remarks>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(Scene scene, IViewSettings settings, IEnumerable<Func<Window, RendererBase>>? rendererFactories = null, ILogger<Window>? logger = null)
    {
        Scene = scene;
        Settings = settings;
        Camera = new(Vector3.One, settings);
        _rendererFactories = rendererFactories?.ToArray() ?? [];
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Window>();

        InitializeWindow();
    }

    /// <summary>
    ///     Initializes the ga
    /// </summary>
    public void InitializeWindow()
    {
        if (_initialized)
            return;

        CurrentWindow = CreateWindow(Settings.WindowOptions);
        CurrentWindow.Update += deltaTime => OnUpdateFrame(new Frame(deltaTime));
        CurrentWindow.Render += deltaTime => RenderFrame(new Frame(deltaTime));
        CurrentWindow.Resize += OnResize;
        CurrentWindow.Load += OnLoad;
        CurrentWindow.Closing += OnClosing;

        _windowInitialized = true;
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
            _logger.LogError(ex, "Error running window: {Message}", ex.Message);
        }
    }

    /// <inheritdoc />
    public override void OnLoad()
    {
        try
        {
            var context = CurrentWindow.CreateOpenGL();
            SetGL(context);

            // Capture the first created GL as the shared GL. This enables resource caches to work across windows
            // when those windows are created with a shared OpenGL context.
            _sharedGl ??= context;

            Input = CurrentWindow.CreateInput();
            CurrentWindow.MakeCurrent();

            SetWindowIcon(PathExtensions.GetAssemblyPath(_iconPath));

            AssignInputEvents();

            _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Load all meshes from the mesh cache
            // MeshService.Instance.LoadMesh("cube", Primitives.Cube.Mesh);

            _renderers = CreateRenderers();

            foreach (var renderer in _renderers)
                renderer.Initialize();

            _imGuiController = new ImGuiController(_gl, CurrentWindow, Input);

            _initialized = true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error loading window: {Message}", ex.Message);
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

            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ToggleWireFrame(Settings.UseWireFrame);

            UseShaders();

            var activeRenderers = _renderers.Where(renderer => Settings.RendererFlags.HasFlag(renderer.RenderFlag))
                                            .OrderBy(renderer => renderer.RenderFlag)
                                            .ToList();

            foreach (var renderer in activeRenderers)
                renderer.Render().GetAwaiter().GetResult();

            AfterRender(frame);

            _imGuiController?.Render();
        }
        catch (Exception ex)
        {
            _logger.LogInformation("{Message}", ex.Message);
        }
    }

    private IReadOnlyList<RendererBase> CreateRenderers()
    {
        if (_rendererFactories.Count > 0)
            return _rendererFactories.Select(factory => factory(this)).ToArray();

        var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(RendererBase)) && !type.IsAbstract);

        return rendererTypes
            .Select(type =>
            {
                var requiredArguments = new object[] { Camera, this, Settings, Scene };
                return (RendererBase)Activator.CreateInstance(type, requiredArguments)!;
            })
            .ToArray();
    }

    /// <summary>
    ///     Toggles the renderer between wireframe and fill mode.
    /// </summary>
    /// <param name="useWireFrame">Determines whether objects should be rendered in wireframe.</param>
    private void ToggleWireFrame(bool useWireFrame)
        => _gl.PolygonMode(GLEnum.FrontAndBack, useWireFrame ? PolygonMode.Line : PolygonMode.Fill);

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
            _logger.LogInformation("FPS: {FrameRate}", frame.FrameRate);

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
            _logger.LogInformation("Input is null. No input events will be assigned.");
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
        _gl.Viewport(size);
        
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
