using Core.Entities.Properties;
using Core.Shaders;

namespace Core.Entities;

public class UIElement : SceneNode
{
    public UIElement(string name)
    {
        Name = name;
        // Mesh = MeshService.Instance.LoadMesh("plane", Primitives.Plane.Mesh);
    }

    private UIShader _uIShader = new UIShader();

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

    public void Initialize()
    {
        _vertexArrayObject = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(_vertexArrayObject);

        InitializeBuffers();
    }

    private void InitializeBuffers()
    {
        var vertexBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        Window.GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        var elementBufferObject = GL.GenBuffer();
        Window.GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        Window.GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
    }

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public override void Render()
    {
        Window.GL.BindVertexArray(_vertexArrayObject);

        _uIShader.Shader.SetMatrix4("model", Transform.ModelMatrix);

        Window.GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}
