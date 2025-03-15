using SharpEngine.Core.Renderers;
using Silk.NET.Windowing;

namespace SharpEngine.Core.Interfaces;

/// <summary>
///     Contains definitions for the settings the game engine supports.
/// </summary>
public interface ISettings
{
    /// <summary>
    ///    Gets or sets whether the object wireframe should be rendered.
    /// </summary>
    public bool UseWireFrame { get; set; }

    /// <summary>
    ///     Gets or sets whether each frames frame rate should be printed to the console.
    /// </summary>
    public bool PrintFrameRate { get; set; }

    /// <summary>Gets or sets which renderer should be enabled for the current window.</summary>
    public RenderFlags RendererFlags { get; set; }

    /// <summary>Gets or sets the settings for a window.</summary>
    WindowOptions WindowOptions { get; set; }
}

/// <inheritdoc cref="ISettings" />
public class DefaultSettings : ISettings
{
    /// <inheritdoc/>
    public bool UseWireFrame { get; set; }

    /// <inheritdoc/>
    public bool PrintFrameRate { get; set; }

    /// <inheritdoc/>
    public RenderFlags RendererFlags { get; set; } = RenderFlags.All;

    /// <inheritdoc/>
    public WindowOptions WindowOptions { get; set; }
}
