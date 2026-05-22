using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Windowing;
using System;
using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;

/// <summary>
///     Contains definitions shared across all renderers.
/// </summary>
public abstract class RendererBase : IDisposable
{
    /// <summary>Gets or sets the settings for the renderer.</summary>
    protected ISettings Settings;

    /// <summary>Gets the window attached to the renderer.</summary>
    protected Window Window { get; private set; } = null!;

    /// <summary>
    ///     Initializes a new instance of <see cref="RendererBase"/>.
    /// </summary>
    /// <param name="settings">The settings for the renderer.</param>
    protected RendererBase(ISettings settings)
    {
        Settings = settings;
    }

    /// <summary>
    ///     Gets the flag for the renderers that this renderer represents.
    /// </summary>
    public abstract RenderFlags RenderFlag { get; }

    /// <summary>
    ///     Attaches the renderer to a window after both have been constructed.
    /// </summary>
    /// <param name="window">The window that owns the renderer.</param>
    public void AttachWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        Window = window;
        OnWindowAttached(window);
    }

    /// <summary>
    ///     Allows derived renderers to react when a window is attached.
    /// </summary>
    /// <param name="window">The attached window.</param>
    protected virtual void OnWindowAttached(Window window) { }

    /// <summary>
    ///     Initializes the renderer.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    ///    Renders the scene.
    /// </summary>
    public abstract Task Render();

    /// <summary>
    ///    Disposes the renderer.
    /// </summary>
    /// <param name="disposing">Whether the renderer should be disposed.</param>
    protected virtual void Dispose(bool disposing) { }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
