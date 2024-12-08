using Core.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace Core;

public class Renderer
{
    private int _vaoModel;
    private int _vaoLamp;

    private Shader _lampShader;
    private Shader _lightingShader;

    private readonly IGame _game;
    private readonly Scene _scene;

    public bool DrawWireFrame { get; set; } = true;

    // Read only once, load into OpenGL buffer once.
    //
    // TODO: Multiple meshes
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

    public Renderer(IGame game, Scene scene)
    {
        _game = game;
        _scene = scene;
    }

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
        _lightingShader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag");
        _lampShader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag");
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

    public void Render(Camera camera)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (DrawWireFrame)
        {
            // Set polygon mode to line to draw wireframe
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

        GL.BindVertexArray(_vaoModel);

        foreach (var node in _scene.Nodes)
        {
            if (node is GameObject gameObject)
            {
                // Perform frustum culling
                if (!IsInViewFrustum(gameObject.BoundingBox, camera))
                {
                    continue;
                }

                gameObject.Render(camera, _game.DirectionalLight, _game.PointLights, _game.SpotLight);
            }
        }

        GL.BindVertexArray(_vaoLamp);

        _lampShader.Use();

        _lampShader.SetMatrix4("view", camera.GetViewMatrix());
        _lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

        // We use a loop to draw all the lights at the proper position
        foreach (var pointLight in _game.PointLights)
        {
            Matrix4 lampMatrix = Matrix4.CreateScale(pointLight.Scale);
            lampMatrix *= Matrix4.CreateTranslation(pointLight.Position);

            _lampShader.SetMatrix4("model", lampMatrix);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        if (DrawWireFrame)
        {
            // Reset polygon mode to fill to draw solid objects
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
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

    public float DistanceToPoint(System.Numerics.Plane plane, Vector3 point)
    {
        var normal = new Vector3(plane.Normal.X, plane.Normal.Y, plane.Normal.Z);
        return Vector3.Dot(normal, point) + plane.D;
    }
}

public class BoundingBox
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }

    public BoundingBox(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
}
