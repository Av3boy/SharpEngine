using Core.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace Core;

/// <summary>
///     Represents the game renderer.
/// </summary>
public class Renderer
{
    private int _vaoModel;
    private int _vaoLamp;

    private Shader _lampShader;
    private Shader _lightingShader;

    private readonly IGame _game;
    private readonly Scene _scene;

    private List<Shader> _shaders = new();

    // Read only once, load into OpenGL buffer once.
    //
    // TODO: Multiple meshes
    // TODO: Create mesh service to keep track of loaded meshes
    // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.
    private static readonly float[] _vertices = GetVertices();

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
    /// <param name="game"></param>
    /// <param name="scene"></param>
    public Renderer(IGame game, Scene scene)
    {
        _game = game;
        _scene = scene;
    }

    /// <summary>
    ///     Initializes the renderer.
    /// </summary>
    public void Initialize()
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
        _lightingShader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
        _lampShader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag", "lamp");
    }

    private void InitializeVertexArrays()
    {
        _vaoModel = GL.GenVertexArray();
        GL.BindVertexArray(_vaoModel);

        var positionLocation = _lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        var normalLocation = _lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

        var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);

        _vaoLamp = GL.GenVertexArray();
        GL.BindVertexArray(_vaoLamp);

        positionLocation = _lampShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);
    }

    /// <summary>
    ///    Renders the scene.
    /// </summary>
    /// <param name="camera">The camera where the scene should be rendered to.</param>
    public void Render(Camera camera)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (ShaderService.Instance.HasShadersToLoad)
        {
            _shaders = ShaderService.Instance.GetAll();
        }

        _shaders.ForEach(shader => shader.Use());

        camera.SetShaderUniforms(_lightingShader);

        if (_game.CoreSettings.UseWireFrame)
        {
            // Set polygon mode to line to draw wireframe
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

        GL.BindVertexArray(_vaoModel);

        RenderSceneNode(_scene.Root, _game.Camera);

        GL.BindVertexArray(_vaoLamp);

        if (_game.CoreSettings.UseWireFrame)
        {
            // Reset polygon mode to fill to draw solid objects
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }

    private void RenderSceneNode(SceneNode node, Camera camera)
    {
        RenderGameObject(node, camera);

        foreach (var child in node.Children)
        {
            RenderSceneNode(child, camera);
        }
    }

    private void RenderGameObject(SceneNode node, Camera camera)
    {
        if (node is GameObject gameObject)
        {
            if (gameObject is Light light)
            {
                switch (light)
                {
                    case DirectionalLight directionalLight:
                        directionalLight.Render(_lightingShader);
                        break;
                    case PointLight pointLight:
                        pointLight.Render(_lightingShader, _lampShader);
                        break;
                    case SpotLight spotLight:
                        spotLight.Render(_lightingShader);
                        break;
                }

                return;
            }

            // Perform frustum culling
            if (!IsInViewFrustum(gameObject.BoundingBox, camera))
                return;

            // TODO: Skip blocks that are behind others relative to the camera

            gameObject.Render(camera);
        }
    }

    private bool IsInViewFrustum(BoundingBox boundingBox, Camera camera)
    {
        var planes = camera.GetFrustumPlanes();

        foreach (var plane in planes)
        {
            if (DistanceToPoint(plane, boundingBox.Min) < 0 && DistanceToPoint(plane, boundingBox.Max) < 0)
            {
                return false;
            }
        }

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

/// <summary>
///     Represents a bounding box of a gameobject.
///     TODO: Move to a separate file and as a property of GameObject.
/// </summary>
public class BoundingBox
{
    /// <summary>
    ///     Gets or sets the minimum point of the bounding box.
    /// </summary>
    public Vector3 Min { get; set; }

    /// <summary>
    ///     Gets or sets the maximum point of the bounding box.
    /// </summary>
    public Vector3 Max { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="min">The minimum point of the box.</param>
    /// <param name="max">The maximum point of the box.</param>
    public BoundingBox(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
}
