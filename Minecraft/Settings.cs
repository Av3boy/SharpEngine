using Core.Interfaces;
using Core.Renderers;

namespace Minecraft;

/// <inheritdoc cref="ISettings" />
public class Settings : ISettings
{
    /// <inheritdoc/>
    public bool UseWireFrame { get; set; }

    /// <inheritdoc/>
    public bool PrintFrameRate { get; set; }

    /// <inheritdoc/>
    public RenderFlags RendererFlags { get; set; } = RenderFlags.All;
}
