using Microsoft.Extensions.Logging;

using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Lights;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using SharpEngine.Telemetry;

using Silk.NET.OpenGL;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plane = System.Numerics.Plane;

namespace SharpEngine.Core.Renderers;

/// <summary>
///     Represents the game renderer.
/// </summary>
public class Renderer : RendererBase
{
    private LampShader _lampShader = null!;
    private LightingShader _lightingShader = null!;

    private readonly CameraView _camera;
    private readonly Scene _scene;
    private readonly ILogger<Renderer> _logger;

    private GL _gl = null!;

    // TODO: #7 Property for specific type of objects
    // No heavy iteration reads for filtering,
    // instead use a notification system from the scene that an item has been removed / added?

    // Read only once, load into OpenGL buffer once.
    // TODO: #5 Multiple meshes

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.Renderer3D;

    /// <summary>
    ///     Initializes a new instance of <see cref="Renderer"/>.
    /// </summary>
    /// <param name="camera">The game the renderer is being used for.</param>
    /// <param name="settings">The settings for the renderer.</param>
    /// <param name="scene">The game scene to be rendered.</param>
    /// <param name="logger">The logger for the renderer.</param>
    public Renderer(CameraView camera, ISettings settings, Scene scene, ILogger<Renderer> logger) : base(settings)
    {
        _camera = camera;
        _scene = scene;
        _logger = logger;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Renderer"/>.
    /// </summary>
    public Renderer(CameraView camera, ISettings settings, Scene scene)
        : this(camera, settings, scene, LoggingExtensions.CreateLogger<Renderer>()) { }

    /// <inheritdoc />
    protected override void OnWindowAttached(Window window)
    {
        _gl = window.GetGL();

        // TODO: #5 These should be refactored out. The minimum build shouldn't need to use these.
        _lightingShader = new LightingShader(_gl);
        _lampShader = new LampShader(_gl);
    }

    /// <inheritdoc />
    public override async Task Render()
    {
        try
        {
            _gl.Enable(EnableCap.DepthTest);

            // Enable image transparency.
            // TODO: #62 Needs testing.
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _camera.SetShaderUniforms(_lightingShader);
            _gl.BindVertexArray(_lightingShader.Vao);

            // Render lights sequentially to avoid allocating a Task per node.
            await TraverseAndAwait(_scene.Root.Children, RenderLight);

            // Render game objects sequentially. Streaming avoids large Task allocations and reduces GC pressure.
            await TraverseAndAwait(_scene.Root.Children, RenderGameObject);

            _gl.BindVertexArray(_lampShader.Vao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }
    }

    private static async Task TraverseAndAwait(IEnumerable<SceneNode> nodes, Func<SceneNode, Task> action)
    {
        foreach (var node in nodes)
        {
            await action(node).ConfigureAwait(false);

            if (node.Children is { Count: > 0 })
                await TraverseAndAwait(node.Children, action).ConfigureAwait(false);
        }
    }

    private Task RenderGameObject(SceneNode node)
    {
        if (node is not GameObject gameObject || node is Light)
            return Task.CompletedTask;

        // TODO: #7 Fix culling for blocks that are partially in view
        // Perform frustum culling
        if (!IsInViewFrustum(gameObject.BoundingBox, _camera))
            return Task.CompletedTask;

        // TODO: #7 Skip blocks that are behind others relative to the camera
        return gameObject.Render(_camera, Window);
        // gameObject.Render(_camera, Window);
    }

    private Task RenderLight(SceneNode node)
    {
        if (node is not Light light)
            return Task.CompletedTask;

        return light.Render(_camera, Window);
    }

    private static bool IsInViewFrustum(BoundingBox boundingBox, CameraView camera)
    {
        if (boundingBox is null)
            return true;

        var planes = camera.GetFrustumPlanes();

        foreach (var plane in planes)
            if (DistanceToPoint(plane, boundingBox.Min) < 0 && DistanceToPoint(plane, boundingBox.Max) < 0)
                return false;

        return true;
    }

    /// <summary>
    ///     Calculates the distance from the given <paramref name="plane"/> to a <paramref name="point"/>.
    /// </summary>
    /// <param name="plane">The origin of where the distance to <paramref name="point"/> should be calculated.</param>
    /// <param name="point">The point the distance to is calculated.</param>
    /// <returns>The distance from <paramref name="plane"/> to <paramref name="point"/>.</returns>
    public static float DistanceToPoint(Plane plane, Vector3 point)
    {
        var normal = new Vector3(plane.Normal.X, plane.Normal.Y, plane.Normal.Z);
        return Vector3.Dot(normal, point) + plane.D;
    }
}
