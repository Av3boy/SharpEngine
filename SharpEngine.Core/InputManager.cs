using Microsoft.Extensions.Logging;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Handlers;
using SharpEngine.Core.Windowing;
using SharpEngine.Telemetry;
using Silk.NET.Input;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEngine.Core;

/// <summary>
///     Handles input events for the application, including mouse and keyboard input. 
///     This class is responsible for managing input contexts and dispatching input events to registered handlers.
/// </summary>
public class InputManager : EngineHandler
{
    private readonly ILogger<InputManager> _logger;

    /// <summary>Gets or sets the input context for the window.</summary>
    public IInputContext? Context { get; protected set; }

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
    ///     Initializes a new instance of the <see cref="InputManager"/>.
    /// </summary>
    /// <param name="logger">The logger for the input manager. When <see langword="null"/>, a default logger will be used.</param>
    public InputManager(ILogger<InputManager>? logger = null) : base(logger)
    {
        _logger = logger ?? LoggingExtensions.CreateLogger<InputManager>();
    }

    /// <summary>
    ///    Sets the input <see cref="Context" /> for the window and assigns input events.
    /// </summary>
    /// <param name="context">The input <see cref="Context" /> to set.</param>
    public virtual void Set(IInputContext context)
    {
        Context = context;
        AssignInputEvents();
    }

    /// <summary>
    ///     Updates the input manager, processing mouse and keyboard events, and invoking the update event.
    /// </summary>
    /// <param name="frame">The current frame information.</param>
    public virtual void Update(Frame frame)
    {
        UpdateMice();
        UpdateKeyboards(frame);
    
        if (Context is not null)
            OnUpdate?.Invoke(frame.FrameTime, Context);
    }

    /// <summary>
    ///    Updates the keyboard input, checking for key presses and invoking the keyboard event handler.
    /// </summary>
    /// <param name="frame">The current frame information.</param>
    protected virtual void UpdateKeyboards(Frame frame)
    {
        var keyboard = Context?.Keyboards[0];
        if (keyboard is not null)
        {
            // if (keyboard.IsKeyPressed(Key.Escape))
            //     CurrentWindow.Close();
    
            OnHandleKeyboard?.Invoke(keyboard, frame.FrameTime);
        }
    }
    
    /// <summary>
    ///     Updates the mouse input, checking for mouse movements and button presses, and invoking the mouse event handler.
    /// </summary>
    public virtual void UpdateMice()
    {
        // TODO: #21 Handle multiple mice?
        var mouse = Context?.Mice[0];
        if (mouse is not null)
        {
            // Camera.UpdateMousePosition((Vector2)mouse.Position);
            OnHandleMouse?.Invoke(mouse);
        }
    }

    /// <summary>
    ///     Assigns input events to the keyboards and mice in the current input <see cref="Context" />.
    /// </summary>
    /// <remarks>
    ///     This method attaches event handlers to the keyboards and mice in the current input <see cref="Context" />.
    ///     If the input <see cref="Context" /> is null, no events will be assigned.
    /// </remarks>
    public virtual void AssignInputEvents()
    {
        if (Context is null)
        {
            _logger.LogInformation("Input is null. No input events will be assigned.");
            return;
        }

        foreach (var keyboard in Context.Keyboards)
            keyboard.KeyDown += KeyDown;

        foreach (var mouse in Context.Mice)
        {
            mouse.Scroll += OnMouseWheel;
            mouse.Click += OnMouseClick;
            mouse.MouseDown += OnMouseDown;

            // if (IsFocused)
            //     mouse.Cursor.CursorMode = CursorMode.Raw;
        }
    }

    /// <summary>
    ///     Handles mouse click events, invoked when a mouse button is clicked.
    /// </summary>
    /// <param name="mouse">The mouse that triggered the event.</param>
    /// <param name="button">The mouse button that was clicked.</param>
    /// <param name="vector">The position of the mouse when the button was clicked.</param>
    public virtual void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector) { }

    /// <summary>
    ///     Handles key down events, invoked when a key is pressed on the keyboard.
    /// </summary>
    /// <param name="keyboard"></param>
    /// <param name="key"></param>
    /// <param name="keyCode"></param>
    public virtual void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        // if (key == Key.Escape)
        //     CurrentWindow.Close();
    }

    /// <summary>
    ///    Handles mouse wheel events, invoked when the mouse wheel is scrolled.
    /// </summary>
    /// <param name="mouse">The mouse that triggered the event.</param>
    /// <param name="sw">The scroll wheel event data.</param>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void OnMouseWheel(IMouse mouse, ScrollWheel sw)
    {
        var direction = sw.Y switch
        {
            > 0 => MouseWheelScrollDirection.Up,
            < 0 => MouseWheelScrollDirection.Down,
            _ => throw new NotImplementedException()
        };

        HandleMouseWheel?.Invoke(direction, sw);
        // Camera.Fov -= sw.Y;
    }

    /// <summary>
    ///   Handles mouse down events, invoked when a mouse button is pressed down.
    /// </summary>
    /// <param name="mouse"></param>
    /// <param name="button"></param>
    public virtual void OnMouseDown(IMouse mouse, MouseButton button)
        => OnButtonMouseDown?.Invoke(mouse, button);

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
