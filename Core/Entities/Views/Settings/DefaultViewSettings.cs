using SharpEngine.Core.Renderers;

using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SharpEngine.Core.Entities.Views.Settings;

/// <summary>
///     Contains the settings for a view.
/// </summary>
public struct DefaultViewSettings : IViewSettings
{
    /// <summary>
    ///     Initializes a new instance of <see cref="ViewSettings"/>.
    /// </summary>
    public DefaultViewSettings()
    {
        MouseSensitivity = 0.2f;
    }

    /// <summary>
    /// Convenience wrapper around creating a new WindowProperties with sensible defaults.
    /// </summary>
    public static ViewSettings Default { get; private set; }

    /// <inheritdoc />
    public float MouseSensitivity { get; set; }

    /// <inheritdoc />
    public bool UseWireFrame { get; set; }

    /// <inheritdoc />
    public bool PrintFrameRate { get; set; }

    /// <inheritdoc />
    public RenderFlags RendererFlags { get; set; } = RenderFlags.All;

    /// <inheritdoc />
    public WindowOptions WindowOptions { get; set; } = WindowOptions.Default with
    {
        Title = "SharpEngine",
        Size = new Vector2D<int>(1280, 720),
    };
}
