using Core.Entities;
using Core.Entities.Properties;
using Core.Interfaces;
using Core.Shaders;
using SharpEngine.Core.Scenes;
using Silk.NET.OpenGL;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Core.Renderers;

/// <summary>
///     Represents the a basic 3D renderer.
/// </summary>
public class Renderer : RendererBase
{
    private uint _vaoModel;
    private uint _vaoLamp;

    private LampShader _lampShader;
    private LightingShader _lightingShader;

    private readonly View _camera;
    private readonly Scene _scene;
    private readonly ISettings _settings;

    // Read only once, load into OpenGL buffer once.
    //
    // TODO: Multiple meshes
    // TODO: Create mesh service to keep track of loaded meshes
    // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.Renderer3D;

    /// <summary>
    ///     Initializes a new instance of <see cref="Renderer"/>.
    /// </summary>
    /// <param name="scene">The scene to be rendered.</param>
    public Renderer(View camera, Scene scene, ISettings settings) : base(settings)
    {
        _camera = camera;
        _scene = scene;
        _settings = settings;
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        InitializeShaders();
        InitializeVertexArrays();
    }

    private void InitializeShaders()
    {
        _lightingShader = new LightingShader();
        _lampShader = new LampShader();
    }

    private void InitializeVertexArrays()
    {
        _vaoModel = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(_vaoModel);

        _lightingShader.SetAttributes();

        _vaoLamp = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(_vaoLamp);

        _lampShader.SetAttributes();
    }

    /// <inheritdoc />
    public override async Task Render()
    {
        try
        {
            Window.GL.Enable(EnableCap.DepthTest);
            
            // TODO: Is this required for basic views
            // _camera.SetShaderUniforms(_lightingShader.Shader);

            Window.GL.BindVertexArray(_vaoModel);
            await _scene.IterateAsync(_scene.Root.Children, RenderGameObject);
            Window.GL.BindVertexArray(_vaoLamp);
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e);
        }
    }

    private async Task RenderGameObject(SceneNode node)
    {
        if (node is GameObject gameObject)
        {
            // TODO: Do this only when using a camera view.
            // TODO: Fix culling for blocks that are partially in view
            // Perform frustum culling
            // if (!IsInViewFrustum(gameObject.BoundingBox, _camera))
            //     return;

            // TODO: Skip blocks that are behind others relative to the camera
            await gameObject.Render();
        }
    }

    private static bool IsInViewFrustum(BoundingBox boundingBox, View camera)
    {
        if (boundingBox is null)
            return true;

        //var planes = camera.GetFrustumPlanes();
        var planes = new List<System.Numerics.Plane>();

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
    public static float DistanceToPoint(System.Numerics.Plane plane, Vector3 point)
    {
        var normal = new Vector3(plane.Normal.X, plane.Normal.Y, plane.Normal.Z);
        return Vector3.Dot(normal, point) + plane.D;
    }
}
