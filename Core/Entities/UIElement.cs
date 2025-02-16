using Core.Entities.Properties;
using Core.Shaders;
using Silk.NET.OpenGL;

namespace Core.Entities;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : SceneNode
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name"></param>
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
    public Mesh Mesh { get; set; }

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
        Window.GL.BindVertexArray(_vertexArrayObject);

        InitializeBuffers();
    }

    private unsafe void InitializeBuffers()
    {
        fixed (float* vertexDataPtr = _vertices)
        {
            var vertexBufferObject = Window.GL.GenBuffer();
            Window.GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
            Window.GL.BufferData(GLEnum.ArrayBuffer, (uint)_vertices.Length * sizeof(float), vertexDataPtr, GLEnum.StaticDraw);
        }

        fixed (uint* indicieDataPtr = _indices)
        {
            var elementBufferObject = Window.GL.GenBuffer();
            Window.GL.BindBuffer(GLEnum.ElementArrayBuffer, elementBufferObject);
            Window.GL.BufferData(GLEnum.ElementArrayBuffer, (uint)_indices.Length * sizeof(uint), indicieDataPtr, GLEnum.StaticDraw);
        }
    }

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public override void Render()
    {
        Window.GL.BindVertexArray(_vertexArrayObject);

        _uIShader.Shader.SetMatrix4("model", Transform.ModelMatrix);

        Window.GL.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}
