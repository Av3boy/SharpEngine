using Core.Interfaces;
using Core.Renderers;
using SharpEngine.Core.Interfaces;
using Silk.NET.Windowing;

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

    /// <inheritdoc/>
    public WindowOptions WindowOptions { get; set; }
}
