using Core.Renderers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SharpEngine.Core.Entities.Views.Settings;

public struct DefaultViewSettings : IViewSettings
{
    public DefaultViewSettings()
    {
        MouseSensitivity = 0.2f;
    }

    public float MouseSensitivity { get; set; }
    public bool UseWireFrame { get; set; }
    public bool PrintFrameRate { get; set; }
    public RenderFlags RendererFlags { get; set; }
    public WindowOptions WindowOptions { get; set; } = WindowOptions.Default with
    {
        Title = "SharpEngine",
        Size = new Vector2D<int>(1280, 720),
    };
    Renderers.RenderFlags ISettings.RendererFlags { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
