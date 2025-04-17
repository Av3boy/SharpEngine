using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;

using Silk.NET.OpenGL;
using System.Numerics;
using System.Threading.Tasks;
using Vector2 = SharpEngine.Core.Numerics.Vector2;

namespace SharpEngine.Core.Entities.UI;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : EmptyNode<Transform2D, Vector2>, IRenderable
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    public UIElement() : this("UIElement") { }

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(string name) : base(name)
    {
        // TODO: #5 Support custom meshes?
        Mesh = MeshService.Instance.LoadMesh(nameof(Primitives.Plane), Primitives.Plane.Mesh);

        Initialize();
    }

    private readonly UIShader _uiShader = new();

    /// <summary>Gets or sets the width of the ui element.</summary>
    public float Width { get; set; } = 10;

    /// <summary>Gets or sets the height of the ui element.</summary>
    public float Height { get; set; } = 10;

    /// <summary>Gets or sets the mesh of the UI element.</summary>
    public Mesh Mesh { get; set; }

    /// <inheritdoc />
    public uint VAO { get; set; }

    /// <inheritdoc />
    public void Initialize()
    {
        VAO = Window.GL.GenVertexArray();
        Bind();

        InitializeBuffers(Mesh);

        _uiShader.Shader?.Use();
        _uiShader.SetAttributes();
    }

    /// <inheritdoc />
    public void Bind()
    {
        Window.GL.BindVertexArray(VAO);
    }

    /// <inheritdoc />
    public void InitializeBuffers(Mesh mesh, bool useMeshVertices = false)
    {
        var vertexBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
        Window.GL.BufferData<float>(GLEnum.ArrayBuffer, mesh.GetVertices(), GLEnum.StaticDraw);

        var elementBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ElementArrayBuffer, elementBufferObject);
        Window.GL.BufferData<uint>(GLEnum.ElementArrayBuffer, mesh.Indices, GLEnum.StaticDraw);
    }

    Matrix4x4 OrthoMatrix = Matrix4x4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public override Task Render(CameraView camera, Window window)
    {
        if (_uiShader.Shader is null)
            return Task.CompletedTask;

        Bind();

        // TODO: #75 These should come from somewhere else.
        const float screenWidth = 1280;
        const float screenHeight = 720;

        _uiShader.Shader.SetFloat("width", Width);
        _uiShader.Shader.SetFloat("height", Height);
        _uiShader.Shader.SetVector2("screenSize", new System.Numerics.Vector2(screenWidth, screenHeight));
        _uiShader.Shader.SetVector2("position", (System.Numerics.Vector2)Transform.Position);
        _uiShader.Shader.SetFloat("rotation", Math.DegreesToRadians(Transform.Rotation.Angle));
        _uiShader.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        _uiShader.Shader.SetMatrix4("orthoMatrix", OrthoMatrix); // Pass the orthographic matrix to the shader

        Window.GL.DrawElements<uint>(PrimitiveType.Triangles, (uint)Mesh.Indices.Length, DrawElementsType.UnsignedInt, []);

        return Task.CompletedTask;
    }
}
