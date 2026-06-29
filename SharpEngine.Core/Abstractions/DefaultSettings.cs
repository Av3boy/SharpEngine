using Microsoft.Extensions.Logging;
using SharpEngine.Core.Renderers;
using Silk.NET.Input;

namespace SharpEngine.Core.Interfaces;

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
    public MouseButton PrimaryButton { get; set; } = MouseButton.Left;

    /// <inheritdoc/>
    public MouseButton SecondaryButton { get; set; } = MouseButton.Right;

    /// <inheritdoc/>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}
