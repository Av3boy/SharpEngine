using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;

using Silk.NET.OpenGL;
using System.Numerics;
using System.Threading.Tasks;
using Vector2 = SharpEngine.Core.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace SharpEngine.Core.Entities.UI;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : EmptyNode<Transform2D, Vector2>
{
    public UIElement() : this("UIElement") { }

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(string name) : base(name)
    {
        Mesh = MeshService.Instance.LoadMesh(nameof(Primitives.Plane), Primitives.Plane.Mesh);

        Initialize();
    }

    private readonly UIShader _uiShader = new();

    /// <summary>Gets or sets the mesh of the UI element.</summary>
    public Mesh Mesh { get; set; }
    //public override Transform2D Transform { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private uint _vertexArrayObject;

    /// <inheritdoc />
    public void Initialize()
    {
        _vertexArrayObject = Window.GL.GenVertexArray();
        Bind();

        InitializeBuffers();

        _uiShader.Shader.Use();
        _uiShader.SetAttributes();
    }

    /// <summary>
    ///     Binds the VAO to the current context.
    /// </summary>
    public void Bind()
    {
        Window.GL.BindVertexArray(_vertexArrayObject);
    }

    private void InitializeBuffers()
    {
        var vertexBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
        Window.GL.BufferData<float>(GLEnum.ArrayBuffer, Mesh.GetVertices(), GLEnum.StaticDraw);

        var elementBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ElementArrayBuffer, elementBufferObject);
        Window.GL.BufferData<uint>(GLEnum.ElementArrayBuffer, Mesh.Indices, GLEnum.StaticDraw);
    }

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public override Task Render(CameraView camera)
    {
        Window.GL.BindVertexArray(_vertexArrayObject);

        // TODO: These should come from somewhere else.
        const float screenWidth = 1280;
        const float screenHeight = 720;

        var orthoMatrix = Matrix4x4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);


        _uiShader.Shader.SetVector2("screenSize", new System.Numerics.Vector2(screenWidth, screenHeight));
        _uiShader.Shader.SetVector2("position", (System.Numerics.Vector2)Transform.Position);
        _uiShader.Shader.SetFloat("rotation", Math.DegreesToRadians(Transform.Rotation.Angle));
        _uiShader.Shader.SetVector2("scale", (System.Numerics.Vector2)Transform.Scale);
        _uiShader.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        _uiShader.Shader.SetMatrix4("orthoMatrix", orthoMatrix); // Pass the orthographic matrix to the shader

        _uiShader.Shader!.SetMatrix4("view", camera.GetViewMatrix());
        _uiShader.Shader!.SetMatrix4("projection", camera.GetProjectionMatrix());
        _uiShader.Shader!.SetVector3("viewPos", camera.Position);


        var pos = new Vector3(Transform.Position.X, Transform.Position.Y, 0);
        var clipSpace = WorldToClipSpace(pos, Transform.ModelMatrix, camera.GetViewMatrix(), camera.GetProjectionMatrix());
        _uiShader.Shader.SetVector4("clipSpace", clipSpace);

        Window.GL.DrawElements<uint>(PrimitiveType.Triangles, (uint)Mesh.Indices.Length, DrawElementsType.UnsignedInt, []);

        return Task.CompletedTask;
    }

    public static Vector4 WorldToClipSpace(Vector3 position, Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
    {
        // Combine all transforms into MVP
        Matrix4x4 mvp = model * view * projection;

        // Convert Vector3 to Vector4 for matrix multiplication
        Vector4 worldPos = new Vector4(position, 1.0f);

        // Transform to clip space (i.e., ready to assign to gl_Position)
        Vector4 clipSpacePos = Vector4.Transform(worldPos, mvp);

        return clipSpacePos;
    }
}
