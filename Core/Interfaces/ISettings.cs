using Core.Renderers;

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
}
