using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;

using Silk.NET.OpenGL;

using System;
using System.Numerics;
using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;

/// <summary>
///     Represents the game renderer.
/// </summary>
public class Renderer : RendererBase
{
    private readonly LampShader _lampShader;
    private readonly LightingShader _lightingShader;

    private readonly CameraView _camera;
    private readonly Scene _scene;

    // Read only once, load into OpenGL buffer once.
    // TODO: Multiple meshes

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.Renderer3D;

    /// <summary>
    ///     Initializes a new instance of <see cref="Renderer"/>.
    /// </summary>
    /// <param name="camera">The game the renderer is being used for.</param>
    /// <param name="settings">The settings for the renderer.</param>
    /// <param name="scene">The game scene to be rendered.</param>
    public Renderer(CameraView camera, ISettings settings, Scene scene) : base(settings)
    {
        _camera = camera;
        _scene = scene;

        // TODO: These should be refactored out. The minimum build shouldn't need to use these.
        _lightingShader = new LightingShader();
        _lampShader = new LampShader();
    }

    /// <inheritdoc />
    public override Task Render()
    {
        return Task.CompletedTask;

        try
        {
            Window.GL.Enable(EnableCap.DepthTest);

            _camera.SetShaderUniforms(_lightingShader.Shader!);
            Window.GL.BindVertexArray(_lightingShader.Vao);

            var gameObjectRenderTasks = _scene.IterateAsync(_scene.Root.Children, RenderGameObject);
            var renderTask = Task.WhenAll(gameObjectRenderTasks);

            Window.GL.BindVertexArray(_lampShader.Vao);

            return renderTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Task.FromException(ex);
        }
    }

    private Task RenderGameObject(SceneNode node)
    {
        if (node is not GameObject gameObject)
            return Task.CompletedTask;

        // TODO: Fix culling for blocks that are partially in view
        // Perform frustum culling
        if (!IsInViewFrustum(gameObject.BoundingBox, _camera))
            return Task.CompletedTask;

        // TODO: Skip blocks that are behind others relative to the camera
        return gameObject.Render();
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
