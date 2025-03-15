using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using System;
using System.Numerics;
using System.Transactions;
using MouseButton = Silk.NET.Input.MouseButton;

namespace SharpEngine.Core;

/// <summary>
///     Represents a class abstraction for the <see cref="IWindow"/> interface.
/// </summary>
public abstract class SilkWindow : IWindow
{
    /// <inheritdoc />
    public virtual IWindowHost? Parent { get; }

    /// <inheritdoc />
    public IMonitor? Monitor { get; set; }

    /// <inheritdoc />
    public bool IsClosing { get; set; }

    public IInputContext Input { get; protected set; }


    /// <inheritdoc />
    public virtual Rectangle<int> BorderSize { get; }

    /// <inheritdoc />
    public bool IsVisible { get; set; }

    /// <inheritdoc />
    public Vector2D<int> Position { get; set; }

    /// <inheritdoc />
    public Vector2D<int> Size { get; set; }

    /// <inheritdoc />
    public string Title
    {
        get => CurrentWindow.Title;
        set => CurrentWindow.Title = value;
    }

    /// <inheritdoc />
    public WindowState WindowState { get; set; }

    /// <inheritdoc />
    public WindowBorder WindowBorder { get; set; }

    /// <inheritdoc />
    public bool TransparentFramebuffer { get; }

    /// <inheritdoc />
    public bool TopMost { get; set; }

    /// <inheritdoc />
    public IGLContext? SharedContext => CurrentWindow.SharedContext;

    /// <inheritdoc />
    public string? WindowClass => CurrentWindow.WindowClass;

    /// <inheritdoc />
    public nint Handle => CurrentWindow.Handle;

    /// <inheritdoc />
    public double Time => CurrentWindow.Time;

    /// <inheritdoc />
    public Vector2D<int> FramebufferSize => CurrentWindow.FramebufferSize;

    /// <inheritdoc />
    public bool IsInitialized => CurrentWindow.IsInitialized;

    /// <inheritdoc />
    public bool ShouldSwapAutomatically { get; set; }

    /// <inheritdoc />
    public bool IsEventDriven { get; set; }
    
    /// <inheritdoc />
    public bool IsContextControlDisabled { get; set; }

    /// <inheritdoc />
    public double FramesPerSecond { get; set; }
    
    /// <inheritdoc />
    public double UpdatesPerSecond { get; set; }

    /// <inheritdoc />
    public GraphicsAPI API { get; }

    /// <inheritdoc />
    public bool VSync { get; set; }

    /// <inheritdoc />
    public VideoMode VideoMode => CurrentWindow.VideoMode;

    /// <inheritdoc />
    public int? PreferredDepthBufferBits => CurrentWindow.PreferredDepthBufferBits;

    /// <inheritdoc />
    public int? PreferredStencilBufferBits => CurrentWindow.PreferredStencilBufferBits;

    /// <inheritdoc />
    public Vector4D<int>? PreferredBitDepth => CurrentWindow.PreferredBitDepth;

    /// <inheritdoc />
    public int? Samples => CurrentWindow.Samples;

    /// <inheritdoc />
    public IGLContext? GLContext => CurrentWindow.GLContext;

    /// <inheritdoc />
    public IVkSurface? VkSurface => CurrentWindow.VkSurface;

    /// <inheritdoc />
    public INativeWindow? Native => CurrentWindow.Native;

    /// <inheritdoc />

    public event Action<Vector2D<int>>? Move;
    
    /// <inheritdoc />
    public event Action<WindowState>? StateChanged;
    
    /// <inheritdoc />
    public event Action<string[]>? FileDrop;
    
    /// <inheritdoc />
    public event Action<Vector2D<int>>? Resize;
    
    /// <inheritdoc />
    public event Action<Vector2D<int>>? FramebufferResize;
    
    /// <inheritdoc />
    public event Action? Closing;
    
    /// <inheritdoc />
    public event Action<bool>? FocusChanged;
    
    /// <inheritdoc />
    public event Action? Load;
    
    /// <inheritdoc />
    public event Action<double>? Update;
    
    /// <inheritdoc />
    public event Action<double>? Render;

    /// <inheritdoc />
    public virtual void Close() => CurrentWindow.Close();

    /// <inheritdoc />
    public virtual void ContinueEvents() => CurrentWindow.ContinueEvents();

    private IWindow? _currentWindow;
    
    /// <summary>Gets or sets the current window.</summary>
    public IWindow CurrentWindow
    {
        get => _currentWindow!;
        set => _currentWindow = value;
    }

    /// <inheritdoc />
    public IWindow CreateWindow(WindowOptions opts)
    {
        CurrentWindow = Silk.NET.Windowing.Window.Create(opts);
        return CurrentWindow;
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing) { }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual void DoEvents() => CurrentWindow.DoEvents();

    /// <inheritdoc />
    public virtual void DoRender() => CurrentWindow.DoRender();

    /// <inheritdoc />
    public virtual void DoUpdate() => CurrentWindow.DoUpdate();

    /// <inheritdoc />
    public virtual void Focus() => CurrentWindow.Focus();

    /// <inheritdoc />
    public virtual void Initialize() => CurrentWindow.Initialize();

    /// <inheritdoc />
    public virtual object Invoke(Delegate d, params object[] args) => new();

    /// <inheritdoc />
    public virtual Vector2D<int> PointToClient(Vector2D<int> point) => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual Vector2D<int> PointToFramebuffer(Vector2D<int> point) => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual Vector2D<int> PointToScreen(Vector2D<int> point) => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual void Reset() => CurrentWindow.Reset();

    public void Run() => CurrentWindow.Run();

    /// <inheritdoc />
    public virtual void Run(Action onFrame) => onFrame();

    /// <inheritdoc />
    public virtual void SetWindowIcon(ReadOnlySpan<RawImage> icons) => CurrentWindow.SetWindowIcon(icons);

    /// <inheritdoc cref="IMouse.Click"/>
    public virtual void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector) { }

    /// <inheritdoc cref="IMouse.MouseDown"/>
    protected virtual void OnMouseDown(IMouse mouse, MouseButton button) { }

    /// <summary>Loads the resources needed to display any objects within the scene / editor.</summary>
    /// <remarks>Called when the window is initialized.</remarks>
    public virtual void OnLoad()
    {
        OnLoaded?.Invoke();
    }

    public event Action? OnLoaded;

    /// <summary>
    ///     Handles operations needed to be executed after the renderers are finished.
    /// </summary>
    /// <param name="deltaTime">Time since the last frame.</param>
    protected virtual void AfterRender(double deltaTime) { }

    /// <summary>
    ///     Handles operations needed to be executed before the renderers are executed.
    /// </summary>
    /// <param name="deltaTime">Time since the last frame.</param>
    protected virtual void PreRender(double deltaTime) { }

    // TODO: Scene unsaved changes warning.
    public virtual void OnClosing() { }
}
