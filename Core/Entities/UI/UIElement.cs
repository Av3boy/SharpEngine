using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;

using Silk.NET.OpenGL;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.UI;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : GameObject
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(string name)
    {
        Name = name;
        Mesh = MeshService.Instance.LoadMesh(nameof(Primitives.Plane), Primitives.Plane.Mesh);

        Initialize();
    }

    private readonly UIShader _uiShader = new();

    /// <summary>Gets or sets the 2D space transformation of the UI element.</summary>
    public new Transform2D Transform { get; set; } = new();

    /// <summary>Gets or sets the mesh of the UI element.</summary>
    public new Mesh Mesh { get; set; }

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
    public override Task Render()
    {
        Window.GL.BindVertexArray(_vertexArrayObject);

        _uiShader.Shader.SetVector2("position", Transform.Position);
        _uiShader.Shader.SetFloat("rotation", Transform.Rotation);
        _uiShader.Shader.SetVector2("scale", Transform.Scale);

        Window.GL.DrawElements<uint>(PrimitiveType.Triangles, (uint)Mesh.Indices.Length, DrawElementsType.UnsignedInt, []);

        return Task.CompletedTask;
    }
}
