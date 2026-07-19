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

    public virtual void Set(IInputContext context)
    {
        Context = context;
        AssignInputEvents();
    }

    public virtual void Update(Frame frame)
    {
        UpdateMice();
        UpdateKeyboards(frame);
    
        if (Context is not null)
            OnUpdate?.Invoke(frame.FrameTime, Context);
    }

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

    // TODO: #21 Input system
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

    public virtual void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector) { }

    public virtual void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        // if (key == Key.Escape)
        //     CurrentWindow.Close();
    }

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

    public virtual void OnMouseDown(IMouse mouse, MouseButton button)
        => OnButtonMouseDown?.Invoke(mouse, button);

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
