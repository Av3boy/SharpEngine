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
    }

    private readonly UIShader _uIShader = new();

    /// <summary>Gets or sets the 2D space transformation of the UI element.</summary>
    public new Transform2D Transform { get; set; } = new()
    {
        //Rotation = 180,
        Scale = new System.Numerics.Vector2(0.5f, 0.5f),
        //Position = new System.Numerics.Vector2(1, 0),
    };

    /// <summary>Gets or sets the mesh of the UI element.</summary>
    public new Mesh Mesh { get; set; }

    private uint _vertexArrayObject;

    /// <inheritdoc />
    public void Initialize()
    {
        _vertexArrayObject = Window.GL.GenVertexArray();
        Bind();

        InitializeBuffers();
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
        
        _uIShader.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        
        Window.GL.DrawElements<uint>(PrimitiveType.Triangles, (uint)Mesh.Indices.Length, DrawElementsType.UnsignedInt, []);

        return Task.CompletedTask;
    }
}
