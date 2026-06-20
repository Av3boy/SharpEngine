using Microsoft.Extensions.Logging;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
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

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    public ParticleRenderer(CameraView camera, ISettings settings, Scene scene)
        : this(camera, settings, scene, LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UIRenderer>())
    {
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    public ParticleRenderer(CameraView camera, ISettings settings, Scene scene, ILogger<UIRenderer> logger) : base(settings)
    {
        // _scene = scene;
        // _camera = camera;
        // _logger = logger;
    }

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.All;

    /// <inheritdoc />
    public override Task Render() => Task.CompletedTask; // => throw new NotImplementedException();
}
