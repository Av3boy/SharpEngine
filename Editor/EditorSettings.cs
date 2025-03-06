using Core.Entities;
using Core.Interfaces;
using Core.Renderers;
using SharpEngine.Core.Entities.Views.Settings;

public class EditorSettings : IViewSettings
{
    /// <inheritdoc />
    public bool UseWireFrame { get; set; }

    /// <inheritdoc />
    public bool PrintFrameRate { get; set; }

    /// <inheritdoc />
    public RenderFlags RendererFlags { get; set; } = RenderFlags.All;

    /// <inheritdoc />
    public Silk.NET.Windowing.WindowOptions WindowOptions { get; set; } = Silk.NET.Windowing.WindowOptions.Default;

    /// <inheritdoc />
    public float MouseSensitivity { get; set; }
}