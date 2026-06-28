using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Interfaces;
using Shader = SharpEngine.Core.Shaders.Shader;

using SharpEngine.Shared.Dto;

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
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Windowing;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private const string _iconPath = "_Resources/icon.png";
    private bool _windowInitialized;
    private bool _initialized;
    private readonly RendererBase[] _registeredRenderers;
    private readonly ILogger<Window> _logger;
    
    private IEnumerable<RendererBase> _renderers = [];
    private ImGuiController? _imGuiController;

    /// <summary>
    ///     Gets or sets the view for the window.
    /// </summary>
    public readonly CameraView Camera;

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
    private GL _gl = null!;

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

    /// <summary>
    ///     Gets the current OpenGL context.
    /// </summary>
    /// <returns>The OpenGL context for this window.</returns>
    public GL GetGL() => _gl;
    private void SetGL(GL gl) => _gl = gl;

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="game">The game instance.</param>
    public Window(Game game)
        : this(game.Scene, game.Camera.Settings, logger: null, renderers: null) { }

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="scene">The scene to be rendered.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(Scene scene, IViewSettings settings)
        : this(scene, settings, logger: null, renderers: null) { }

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="camera">The camera the window should render from.</param>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(CameraView camera, Scene scene, IViewSettings settings)
        : this(camera, scene, settings, logger: null, renderers: null) { }

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <param name="camera">The camera the window should render from.</param>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    /// <param name="logger">The logger for the window. When <see langword="null"/>, a default logger will be used.</param>
    /// <param name="renderers">The renderers for the window.</param>
    public Window(CameraView camera, Scene scene, IViewSettings settings, ILogger<Window>? logger = null, IEnumerable<RendererBase>? renderers = null)
    {
        // TODO:
        // Should the developer need to call for a window initialization?
        // Meaning should should we move this project loading part to a separate function?

        Scene = scene;
        Settings = settings;
        Camera = camera;
        _registeredRenderers = renderers?.ToArray() ?? [];
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Window>();

        CheckEngineVersion();

        // NOTE: Window initialization is intentionally not performed automatically here.
        // Call InitializeWindow() explicitly when ready to create the underlying native window and load resources.
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <remarks>
    ///     The new window is initialized without a dedicated camera.
    /// </remarks>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    /// <param name="logger">The logger for the window. When <see langword="null"/>, a default logger will be used.</param>
    /// <param name="renderers">The renderers for the window.</param>
    public Window(Scene scene, IViewSettings settings, ILogger<Window>? logger = null, IEnumerable<RendererBase>? renderers = null)
    {
        Scene = scene;
        Settings = settings;
        Camera = new(Vector3.One, settings);
        _registeredRenderers = renderers?.ToArray() ?? [];
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Window>();

        // NOTE: Window initialization is intentionally not performed automatically here.
        // Call InitializeWindow() explicitly when ready to create the underlying native window and load resources.
    }

    private void CheckEngineVersion()
    {
        var project = new ProjectDto();
        var currentAssemlyVersion = typeof(Window).Assembly.GetVersion();

        if (currentAssemlyVersion != project.EngineVersion)
            _logger.LogWarning("The current engine version ({CurrentVersion}) does not match the project engine version ({ProjectVersion}). This may lead to unexpected behavior.", currentAssemlyVersion, project.EngineVersion);
    }

    /// <inheritdoc />
    public override void Initialize()
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

        // Ensure the underlying native window is created and initialized.
        // SilkWindow.Initialize calls CurrentWindow.Initialize(); call it here to
        // complete the initialization sequence (create GL context, input, etc.).
        base.Initialize();
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

            // TODO: Skip calling this for secondary windows?
            CurrentWindow.MakeCurrent();

            SetWindowIcon(PathExtensions.GetAssemblyPath(_iconPath));

            AssignInputEvents();

            _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _renderers = CreateRenderers();

            foreach (var renderer in _renderers)
            {
                renderer.AttachWindow(this);
                renderer.Initialize();
            }

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
        if (!_initialized)
            throw new InvalidOperationException("Window has not been initialized. Call Initialize() before rendering.");

        try
        {
            PreRender(frame);

            _imGuiController?.Update((float)frame.FrameTime);

            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ToggleWireFrame(Settings.UseWireFrame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during pre-rendering.");
        }

        try
        {
            UseShaders();

            var activeRenderers = _renderers.Where(renderer => Settings.RendererFlags.HasFlag(renderer.RenderFlag))
                                            .OrderBy(renderer => renderer.RenderFlag)
                                            .ToList();

            foreach (var renderer in activeRenderers)
                renderer.Render().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during rendering.");
        }

        try
        {
            AfterRender(frame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during post render.");
        }

        try
        {
            _imGuiController?.Render();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while rendering ImGui interface.");
        }
    }

    private RendererBase[] CreateRenderers()
    {
        // Return the renderers that have been registered
        if (_registeredRenderers.Length > 0)
            return _registeredRenderers;

        // When no renderers have been registered, fall back to use all that are discoverable.
        var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(RendererBase)) && !type.IsAbstract);

        return [.. rendererTypes
            .Select(type =>
            {
                var requiredArguments = new object[] { Camera, Settings, Scene };
                return (RendererBase)Activator.CreateInstance(type, requiredArguments)!;
            })];
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
            Camera.UpdateMousePosition((Vector2)mouse.Position);
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
