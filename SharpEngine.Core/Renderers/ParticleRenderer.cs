using SharpEngine.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;

/// <summary>
///     Renders particle effects.
/// </summary>
public class ParticleRenderer : RendererBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ParticleRenderer"/>.
    /// </summary>
    /// <param name="settings">The settings for the particle renderer.</param>
    public ParticleRenderer(ISettings settings) : base(settings)
    {
    }

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.All;

    /// <inheritdoc />
    public override Task Render() => throw new NotImplementedException();
}
