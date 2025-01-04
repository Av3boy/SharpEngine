using Core.Interfaces;
using Core.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace Core.Renderers;

/// <summary>
///     Represents the game renderer.
/// </summary>
public class Renderer : RendererBase
{
    private int _vaoModel;
    private int _vaoLamp;

    private LampShader _lampShader;
    private LightingShader _lightingShader;

    private readonly IGame _game;
    private readonly Scene _scene;

    // Read only once, load into OpenGL buffer once.
    //
    // TODO: Multiple meshes
    // TODO: Create mesh service to keep track of loaded meshes
    // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.
    private static readonly float[] _vertices = GetVertices();

    public override RenderFlags RenderFlag => RenderFlags.Renderer3D;

    private static float[] GetVertices()
    {
        var mesh = Primitives.Cube.Mesh; // Mesh is identical for all cubes
        var vertices = new List<float>();

        for (int i = 0; i < mesh.Vertices.Length / 3; i++)
        {
            var vertexIndex = i * 3;

            vertices.Add(mesh.Vertices[vertexIndex]);
            vertices.Add(mesh.Vertices[vertexIndex + 1]);
            vertices.Add(mesh.Vertices[vertexIndex + 2]);

            var normalIndex = i * 3;
            vertices.Add(mesh.Normals[normalIndex]);
            vertices.Add(mesh.Normals[normalIndex + 1]);
            vertices.Add(mesh.Normals[normalIndex + 2]);

            var texCoordIndex = i * 2;
            vertices.Add(mesh.TextureCoordinates[texCoordIndex]);
            vertices.Add(mesh.TextureCoordinates[texCoordIndex + 1]);
        }

        return vertices.ToArray();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Renderer"/>.
    /// </summary>
    /// <param name="game">The game the renderer is being used for.</param>
    /// <param name="scene">The game scene to be rendered.</param>
    public Renderer(IGame game, Scene scene) : base(game.CoreSettings)
    {
        _game = game;
        _scene = scene;
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        InitializeBuffers();
        InitializeShaders();
        InitializeVertexArrays();
    }

    private static void InitializeBuffers()
    {
        var vertexBufferObject = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
    }

    private void InitializeShaders()
    {
        _lightingShader = new LightingShader();
        _lampShader = new LampShader();
    }

    private void InitializeVertexArrays()
    {
        _vaoModel = GL.GenVertexArray();
        GL.BindVertexArray(_vaoModel);

        _lightingShader.SetAttributes();

        _vaoLamp = GL.GenVertexArray();
        GL.BindVertexArray(_vaoLamp);

        _lampShader.SetAttributes();
    }

    /// <inheritdoc />
    public override void Render2()
    {
        _game.Camera.SetShaderUniforms(_lightingShader.Shader);



        GL.BindVertexArray(_vaoModel);

        _scene.Iterate(_scene.Root.Children, RenderGameObject);

        GL.BindVertexArray(_vaoLamp);
    }

    private void RenderGameObject(SceneNode node)
    {
        if (node is GameObject gameObject)
        {
            // TODO: Fix culling for blocks that are partially in view
            // Perform frustum culling
            if (!IsInViewFrustum(gameObject.BoundingBox, _game.Camera))
                return;

            // TODO: Skip blocks that are behind others relative to the camera
            gameObject.Render();
        }
    }

    private static bool IsInViewFrustum(BoundingBox boundingBox, Camera camera)
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
    public static float DistanceToPoint(System.Numerics.Plane plane, Vector3 point)
    {
        var normal = new Vector3(plane.Normal.X, plane.Normal.Y, plane.Normal.Z);
        return Vector3.Dot(normal, point) + plane.D;
    }
}
