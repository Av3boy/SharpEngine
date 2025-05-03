using SharpEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;
public class ParticleRenderer : RendererBase
{
    public ParticleRenderer(ISettings settings) : base(settings)
    {
    }

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.Renderer3D;

    /// <inheritdoc />
    public override Task Render() => throw new NotImplementedException();
}
