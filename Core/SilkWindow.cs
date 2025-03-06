using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using System;
using System.Numerics;
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

    /// <inheritdoc />
    public virtual Rectangle<int> BorderSize { get; }

    /// <inheritdoc />
    public bool IsVisible { get; set; }

    /// <inheritdoc />
    public Vector2D<int> Position { get; set; }

    /// <inheritdoc />
    public Vector2D<int> Size { get; set; }

    /// <inheritdoc />
    public abstract string Title { get; set; }

    /// <inheritdoc />
    public WindowState WindowState { get; set; }

    /// <inheritdoc />
    public WindowBorder WindowBorder { get; set; }

    /// <inheritdoc />
    public bool TransparentFramebuffer { get; }

    /// <inheritdoc />
    public bool TopMost { get; set; }

    /// <inheritdoc />
    public IGLContext? SharedContext { get; }

    /// <inheritdoc />
    public string? WindowClass { get; }

    /// <inheritdoc />
    public nint Handle { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc />
    public Vector2D<int> FramebufferSize { get; }

    /// <inheritdoc />
    public bool IsInitialized { get; }

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
    public VideoMode VideoMode { get; }

    /// <inheritdoc />
    public int? PreferredDepthBufferBits { get; }

    /// <inheritdoc />
    public int? PreferredStencilBufferBits { get; }

    /// <inheritdoc />
    public Vector4D<int>? PreferredBitDepth { get; }

    /// <inheritdoc />
    public int? Samples { get; }

    /// <inheritdoc />
    public IGLContext? GLContext { get; }

    /// <inheritdoc />
    public IVkSurface? VkSurface { get; }

    /// <inheritdoc />
    public INativeWindow? Native { get; }

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
    public virtual void Close() { }

    /// <inheritdoc />
    public virtual void ContinueEvents() { }

    private IWindow? _currentWindow;

    /// <summary>Gets or sets the current window.</summary>
    public IWindow CurrentWindow
    {
        get => _currentWindow ?? this;
        set => _currentWindow = value;
    }

    /// <inheritdoc />
    public virtual IWindow CreateWindow(WindowOptions opts)
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
    public virtual void DoEvents() { }

    /// <inheritdoc />
    public virtual void DoRender() { }

    /// <inheritdoc />
    public virtual void DoUpdate() { }

    /// <inheritdoc />
    public virtual void Focus() { }

    /// <inheritdoc />
    public virtual void Initialize() { }

    /// <inheritdoc />
    public virtual object Invoke(Delegate d, params object[] args) => new();

    /// <inheritdoc />
    public virtual Vector2D<int> PointToClient(Vector2D<int> point) => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual Vector2D<int> PointToFramebuffer(Vector2D<int> point) => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual Vector2D<int> PointToScreen(Vector2D<int> point) => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual void Reset() { }

    /// <inheritdoc />
    public virtual void Run(Action onFrame) { }

    /// <inheritdoc />
    public virtual void SetWindowIcon(ReadOnlySpan<RawImage> icons) { }

    /// <inheritdoc cref="IMouse.Click"/>
    public virtual void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector) { }

    /// <inheritdoc cref="IMouse.MouseDown"/>
    protected virtual void OnMouseDown(IMouse mouse, MouseButton button) { }

    /// <summary>Loads the resources needed to display any objects within the scene / editor.</summary>
    /// <remarks>Called when the window is initialized.</remarks>
    public abstract void OnLoad();

    /// <summary>
    ///     Handles operations needed to be executed after the renderes are finished.
    /// </summary>
    /// <param name="deltaTime">Time since the last frame.</param>
    protected virtual void AfterRender(double deltaTime) { }

    /// <summary>
    ///     Handles operations needed to be executed before the renderes are executed.
    /// </summary>
    /// <param name="deltaTime">Time since the last frame.</param>
    protected virtual void PreRender(double deltaTime) { }
}
