using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Renderers;

namespace SharpEngine.Editor;

/// <summary>
///     Contains settings for the editor.
/// </summary>
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