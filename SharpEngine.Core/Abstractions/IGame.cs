using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;
using Silk.NET.Input;
using System.Collections.Generic;
using System;

namespace SharpEngine.Core.Interfaces;

/// <summary>
///     Contains definitions the Game class must implement.
///     To consider: move into a abstract class so that the game doesn't necessarily have to implement all methods.
/// </summary>
public abstract class Game
{
    private readonly List<Window> _windows = [];
    private Window? _currentWindow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Game" />.
    /// </summary>
    /// <remarks>
    ///     Sets up a default scene and camera.
    /// </remarks>
    public Game()
    {
        Scene = new Scene();
        Camera = new(new Vector3(0), new DefaultViewSettings());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Game" />.
    /// </summary>
    /// <remarks>
    ///     Sets up a default camera.
    /// </remarks>
    public Game(Scene scene)
    {
        Scene = scene;
        Camera = new(new Vector3(0), new DefaultViewSettings());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Game" />.
    /// </summary>
    public Game(Scene scene, CameraView camera)
    {
        Scene = scene;
        Camera = camera;
    }

    /// <summary>
    ///     Gets or sets the core settings of the game.
    /// </summary>
    public ISettings CoreSettings { get; init; } = new DefaultSettings();

    /// <summary>
    ///     Gets or sets the camera of the game.
    /// </summary>
    public CameraView Camera { get; set; } = CameraView.CreateDefault();

    /// <summary>
    ///     Gets or sets the scene loaded in the game.
    /// </summary>
    public Scene Scene { get; init; }

    /// <summary>
    ///     Gets the main window.
    /// </summary>
    public Window Window
    {
        get => _currentWindow ?? throw new InvalidOperationException("The game window has not been assigned yet.");
    }

    /// <summary>
    ///     Gets the windows attached to the game.
    /// </summary>
    public IReadOnlyCollection<Window> Windows => _windows;

    /// <summary>
    ///     Attaches a window to the game.
    /// </summary>
    /// <param name="window">The window to attach.</param>
    public virtual void AttachWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (!_windows.Contains(window))
            _windows.Add(window);

        _currentWindow ??= window;
    }

    /// <summary>
    ///     Sets the currently active window for the game.
    /// </summary>
    /// <param name="window">The active window.</param>
    public virtual void UseWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (!_windows.Contains(window))
            _windows.Add(window);

        _currentWindow = window;
    }

    /// <summary>
    ///     Executed when a mouse button is pressed.
    /// </summary>
    public virtual void HandleMouseDown(IMouse mouse, MouseButton button) { }

    /// <summary>
    ///     A method executed when the mouse state is changed
    /// </summary>
    /// <param name="mouse">The state of the mouse on the current frame.</param>
    public virtual void HandleMouse(IMouse mouse) { }

    /// <summary>
    ///     Gets or sets the event executed when the mouse wheel is scrolled.
    /// </summary>
    /// <param name="direction">The direction the wheel was scrolled.</param>
    /// <param name="scrollWheel">Contains information about the scroll wheel changes during the current frame.</param>
    public virtual void HandleMouseWheel(MouseWheelScrollDirection direction, ScrollWheel scrollWheel) { }

    /// <summary>
    ///     Executed when a key is pressed.
    /// </summary>
    /// <param name="input">The state of the keyboard on the current frame.</param>
    /// <param name="deltaTime">The time since the last frame.</param>
    public virtual void HandleKeyboard(IKeyboard input, double deltaTime) { }

    /// <summary>
    ///     Executed when the game is first loaded.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    ///    Executed each frame.
    /// </summary>
    public virtual void Update(double deltaTime, IInputContext input) { }

    public virtual void OnAfterRender(Frame frame) { }
}
