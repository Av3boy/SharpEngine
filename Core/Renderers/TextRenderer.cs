using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;
internal class TextRenderer : RendererBase
{
    public TextRenderer(CameraView camera, ISettings settings, Scene scene) : base(settings)
    {
    }

    public override RenderFlags RenderFlag => RenderFlags.Text;

    /// <inheritdoc />
    public override Task Render()
    {
        return Task.CompletedTask;
    }
}
