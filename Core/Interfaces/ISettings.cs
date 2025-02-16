using Core.Renderers;
using Silk.NET.Windowing;

namespace Core.Interfaces;

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

    public RenderFlags RendererFlags { get; set; }

    public WindowOptions WindowOptions { get; set; }
}
