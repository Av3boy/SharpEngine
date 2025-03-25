using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;

using Silk.NET.OpenGL;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : SceneNode
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(string name)
    {
        Name = name;
        // Mesh = MeshService.Instance.LoadMesh("plane", Primitives.Plane.Mesh);
    }

    private readonly UIShader _uIShader = new();

    /// <summary>Gets or sets the 2D space transformation of the UI element.</summary>
    public Transform2D Transform { get; set; } = new()
    {
        Rotation = 180
    };

    // TODO: Use actual mesh
    /// <summary>Gets or sets the mesh of the UI element.</summary>
    public Mesh Mesh { get; set; } // = new();

    private readonly float[] _vertices =
    [
        // Position         Texture coordinates
         0.5f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f,  // top right
         0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // top left
    ];

    private readonly uint[] _indices =
    [
       0, 1, 3,
       1, 2, 3
    ];

    private uint _vertexArrayObject;

    /// <inheritdoc />
    public void Initialize()
    {
        _vertexArrayObject = Window.GL.GenVertexArray();
        Bind();

        InitializeBuffers();
    }

    public void Bind()
    {
        Window.GL.BindVertexArray(_vertexArrayObject);
    }

    private void InitializeBuffers()
    {
        var vertexBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
        Window.GL.BufferData<float>(GLEnum.ArrayBuffer, _vertices, GLEnum.StaticDraw);

        var elementBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ElementArrayBuffer, elementBufferObject);
        Window.GL.BufferData<uint>(GLEnum.ElementArrayBuffer, _indices, GLEnum.StaticDraw);
    }

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public unsafe override Task Render()
    {
        Window.GL.BindVertexArray(_vertexArrayObject);
        
        _uIShader.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        
        Window.GL.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, null);

        return Task.CompletedTask;
    }
}
