using SharpEngine.Core.Interfaces;

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
    ///     Initializes the renderer.
    /// </summary>
    public abstract void Initialize();

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
