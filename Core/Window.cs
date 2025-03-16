﻿using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using Silk.NET.Core;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MouseButton = Silk.NET.Input.MouseButton;
using Shader = SharpEngine.Core.Shaders.Shader;

namespace SharpEngine.Core;

/// <summary>
///     Represents the game window.
/// </summary>
public class Window : SilkWindow
{
    private readonly ISettings _settings;
    private readonly CameraView _camera;

    private bool _initialized;
    private IEnumerable<RendererBase> _renderers = [];
    private ImGuiController? _imGuiController;

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

    // TODO: Use this method.
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
        _settings = settings;
        _camera = camera;

        Initialize(settings.WindowOptions);
    }

    /// <summary>
    ///     Initializes a new window without a dedicated camera.
    /// </summary>
    /// <param name="scene">Contains the game scene.</param>
    /// <param name="settings">The settings for the window.</param>
    public Window(Scene scene, IViewSettings settings)
    {
        Scene = scene;
        _settings = settings;
        _camera = new(Vector3.One, new DefaultViewSettings());

        Initialize(settings.WindowOptions);
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
        try
        {
            base.Run(onFrame);
            Run();
        }
        catch (Exception ex)
        {
            Debug.LogInformation("Error running window: " + ex.Message, ex);
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

            // TODO: Set cursor shape
            // CursorShape = CursorShape.Hand;

            // Load all meshes from the mesh cache
            MeshService.Instance.LoadMesh("cube", Primitives.Cube.Mesh);

            // Using reflection, find all renderers that implement the RendererBase.
            var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(RendererBase)) && !type.IsAbstract);

            foreach (var type in rendererTypes)
            {
                // Make sure the renderer has the correct constructor parameters!
                // TODO: The static reference to the context will not work when multiple windows are implemented, since the context will be different.
                var requiredArguments = new object[] { _camera, _settings, Scene };
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

            ToggleWireFrame(_settings.UseWireFrame);

            UseShaders();

            var renderTasks = _renderers.Where(renderer => _settings.RendererFlags.HasFlag(renderer.RenderFlag))
                                        .Select(renderer => renderer.Render())
                                        .ToList();

            Task.WaitAll([.. renderTasks]);

            // TODO: This call causes flickering in the new framework. Investigate why.
            // _window.SwapBuffers();

            AfterRender(deltaTime);

            _imGuiController?.Render();
        }
        catch (Exception ex)
        {
            Debug.LogInformation(ex.Message);
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
    protected void OnUpdateFrame(double deltaTime)
    {
        while (!_initialized)
        {
            // Wait for the window to be initialized.
        }

        // TODO: GLFW_ISFOCUSED
        // if (!_window.IsFocused)
        //     return;

        if (_settings.PrintFrameRate)
            Console.WriteLine($"FPS: {1f / deltaTime}");

        // TODO: Handle multiple mice?
        var mouse = Input?.Mice[0];
        if (mouse is not null)
        {
            _camera.UpdateMousePosition(mouse.Position);
            OnHandleMouse?.Invoke(mouse);
        }

        var keyboard = Input?.Keyboards[0];
        if (keyboard is not null)
        {
            if (keyboard.IsKeyPressed(Key.Escape))
                CurrentWindow.Close();

            OnHandleKeyboard?.Invoke(keyboard, deltaTime);
        }

        if (Input is not null)
            OnUpdate?.Invoke(deltaTime, Input);
    }

    // TODO: #21 Input system
    private void AssignInputEvents()
    {
        if (Input is null)
        {
            Debug.LogInformation("Input is null. No input events will be assigned.");
            return;
        }

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
        _camera.Fov -= sw.Y;
    }

    /// <inheritdoc />
    protected void OnResize(Vector2D<int> size)
    {
        GL.Viewport(size);
        _camera.AspectRatio = size.X / size.Y;
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
        // TODO: Do we need to clear anything from e.g. the GPU when we change change the scene?
        Scene = scene;
    }
}
