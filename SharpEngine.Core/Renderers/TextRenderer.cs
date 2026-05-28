using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;

using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;

/// <summary>
///     A renderer responsible for rendering text in the scene.
/// </summary>
internal class TextRenderer : RendererBase
{
    /// <summary>
    ///     Initializes a new instance of the TextRenderer class with the specified camera, settings, and scene.
    /// </summary>
    /// <param name="camera">CameraView that provides view and projection information for rendering.</param>
    /// <param name="settings">ISettings that configures renderer behavior.</param>
    /// <param name="scene">Scene that contains renderable text elements.</param>
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
