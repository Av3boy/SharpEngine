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
using SharpEngine.Telemetry;

namespace SharpEngine.Core.Windowing;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private bool _windowInitialized;
    private bool _initialized;
    private readonly ILogger<Window> _logger;
    
    private IEnumerable<RendererBase> _renderers = [];
    private ImGuiController? _imGuiController;

    public InputManager _inputManager;
    public ShaderManager _shaderManager;
    public RendererManager _rendererManager;

    /// <summary>
    ///     Gets or sets the view for the window.
    /// </summary>
    public readonly CameraView Camera;

    /// <summary>
    ///     The scene that is currently being rendered.
    /// </summary>
    protected Scene Scene { get; private set; }

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
    ///     Gets the current OpenGL context.
    /// </summary>
    /// <returns>The OpenGL context for this window.</returns>
    public GL GetGL() => _gl;

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
        : this(scene, settings, camera, logger, renderers) { }

    /// <summary>
    ///     Initializes a new instance of <see cref="Window"/>.
    /// </summary>
    /// <remarks>
    ///     The new window is initialized without a dedicated camera.
    /// </remarks>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    /// <param name="camera">The camera the window should render from. When <see langword="null"/>, a default camera will be used.</param>
    /// <param name="logger">The logger for the window. When <see langword="null"/>, a default logger will be used.</param>
    /// <param name="renderers">The renderers for the window.</param>
    public Window(Scene scene, IViewSettings settings, CameraView? camera = null, ILogger<Window>? logger = null, IEnumerable<RendererBase>? renderers = null)
    {
        Scene = scene;
        Settings = settings;
        Camera = camera ?? new(Vector3.One, settings);
        _logger = logger ?? LoggingExtensions.CreateLogger<Window>();

        // TODO:
        // Should the developer need to call for a window initialization?
        // Meaning should should we move this project loading part to a separate function?

        _inputManager = new InputManager();
        _shaderManager = new ShaderManager(LoggingExtensions.CreateLogger<ShaderManager>());
        _rendererManager = new RendererManager(settings, renderers);

        // NOTE: Window initialization is intentionally not performed automatically here.
        // Call InitializeWindow() explicitly when ready to create the underlying native window and load resources.
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
        ArgumentNullException.ThrowIfNull(CurrentWindow, nameof(CurrentWindow));

        try
        {
            SetGL();

            var context = CurrentWindow.CreateInput();
            _inputManager.Set(context);

            CurrentWindow.MakeCurrent();

            SetWindowIcon();

            _gl.ClearColor(BackgroundColor.X, BackgroundColor.Y, BackgroundColor.Z, BackgroundColor.W);

            _renderers = _rendererManager.CreateRenderers(Camera, Scene);

            foreach (var renderer in _renderers)
            {
                renderer.AttachWindow(this);
                renderer.Initialize();
            }

            _imGuiController = new ImGuiController(_gl, CurrentWindow, _inputManager.Context);

            _initialized = true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error loading window: {Message}", ex.Message);
        }

        base.OnLoad();
    }

    private void SetGL()
    {
        var context = CurrentWindow.CreateOpenGL();
        _gl = context;

        // Capture the first created GL as the shared GL.
        // This enables resource caches to work across windows
        // when those windows are created with a shared OpenGL context.
        _sharedGl ??= context;
    }

    /// <summary>
    ///    Renders the current view and all specified renderers.
    /// </summary>
    /// <param name="frame">Contains information about the previous frame.</param>
    protected void RenderFrame(Frame frame)
    {
        if (!_initialized || CurrentWindow == null)
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
            _shaderManager.UseShaders();
            _rendererManager.Run();
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

    /// <summary>
    ///     Toggles the renderer between wireframe and fill mode.
    /// </summary>
    /// <param name="useWireFrame">Determines whether objects should be rendered in wireframe.</param>
    private void ToggleWireFrame(bool useWireFrame)
        => _gl.PolygonMode(GLEnum.FrontAndBack, useWireFrame ? PolygonMode.Line : PolygonMode.Fill);

    /// <summary>
    ///     Called when a window frame is rendered.
    /// </summary>
    /// <param name="frame">The frame information.</param>
    protected void OnUpdateFrame(Frame frame)
    {
        while (!_initialized)
        {
            // Wait for the window to be initialized.
        }

        if (Settings.PrintFrameRate)
            _logger.LogInformation("FPS: {FrameRate}", frame.FrameRate);

        _inputManager.Update(frame);
    }

    /// <summary>
    ///     Called when the window is resized.
    /// </summary>
    /// <param name="size">The new size of the window.</param>
    protected virtual void OnResize(Vector2D<int> size)
    {
        _gl.Viewport(size);

        if (size != Vector2D<int>.Zero)
            Camera.AspectRatio = (float)size.X / size.Y;
    }

    /// <summary>
    ///     Sets the current scene.
    /// </summary>
    /// <param name="scene">The contents of the new scene.</param>
    protected virtual void SetScene(Scene scene)
    {
        // TODO: #92 Do we need to clear anything from e.g. the GPU when we change change the scene?
        Scene = scene;
    }
}
