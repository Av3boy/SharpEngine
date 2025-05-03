using ObjLoader.Loader.Data.Elements;
using SharpEngine.Core.Attributes;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using Silk.NET.OpenGL;
using Tutorial;

using Texture2 = SharpEngine.Core.Components.Properties.Textures.Texture;

namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Represents a game object mesh, which is a collection of vertices, normals, texture coordinates, and indices
///     used to define the shape and appearance of a 3D object.
/// </summary>
public class Mesh : IDisposable
{
    private bool disposedValue;

    /// <summary>
    ///     Gets or sets an identifying name for the mesh.
    /// </summary>
    public string Name { get; set; } = "New Mesh";

    /// <summary>
    ///     Gets or sets the mesh vertices as an array of floats.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Vertices { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh vertices as a list of <see cref="Vertex"/> objects.
    /// </summary>
    public List<Vertex> Vertices2 { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh normals as an array of floats.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Normals { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh normals as a list of <see cref="Normal"/> objects.
    /// </summary>
    public List<Normal> Normals2 { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates as an array of floats.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] TextureCoordinates { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates as a list of <see cref="TextureCoordinate"/> objects.
    /// </summary>
    public List<TextureCoordinate> TextureCoordinates2 { get; set; } = [];

    /// <summary>
    ///     Gets or sets the indices of the mesh as an array of unsigned integers.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public uint[] Indices { get; set; } = [];

    /// <summary>
    ///     Gets or sets the groups of faces in the mesh.
    /// </summary>
    public List<Group> Groups { get; set; } = [];

    /// <summary>
    ///     Gets or sets the materials used by the mesh.
    /// </summary>
    public List<Material> Materials { get; set; } = [];

    /// <summary>
    ///     Gets or sets the textures used by the mesh.
    /// </summary>
    public IReadOnlyList<Texture2> Textures { get; set; }

    /// <summary>
    ///     Gets or sets the legacy textures used by the mesh.
    /// </summary>
    public IReadOnlyList<Texture> Textures_Old { get; set; }

    /// <summary>
    ///     Gets or sets the Vertex Array Object (VAO) for the mesh.
    /// </summary>
    public VertexArrayObject<float, uint> VAO { get; set; }

    /// <summary>
    ///     Gets or sets the Vertex Buffer Object (VBO) for the mesh.
    /// </summary>
    public BufferObject<float> VBO { get; set; }

    /// <summary>
    ///     Gets or sets the Element Buffer Object (EBO) for the mesh.
    /// </summary>
    public BufferObject<uint> EBO { get; set; }

    /// <summary>
    ///     Gets the OpenGL context associated with the mesh.
    /// </summary>
    public GL GL { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Mesh"/> class with the specified OpenGL context, vertices, indices, and textures.
    /// </summary>
    /// <param name="gl">The OpenGL context.</param>
    /// <param name="vertices">The vertices of the mesh.</param>
    /// <param name="indices">The indices of the mesh.</param>
    /// <param name="textures">The textures of the mesh.</param>
    public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture2> textures)
    {
        GL = gl;
        Vertices = vertices;
        Indices = indices;
        Textures = textures;
        SetupMesh();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Mesh"/> class with the specified OpenGL context, vertices, indices, and legacy textures.
    /// </summary>
    /// <param name="gl">The OpenGL context.</param>
    /// <param name="vertices">The vertices of the mesh.</param>
    /// <param name="indices">The indices of the mesh.</param>
    /// <param name="textures">The legacy textures of the mesh.</param>
    public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture> textures)
    {
        GL = gl;
        Vertices = vertices;
        Indices = indices;
        Textures_Old = textures;
        SetupMesh();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Mesh"/> class with the specified OpenGL context.
    /// </summary>
    /// <param name="gl">The OpenGL context.</param>
    public Mesh(GL gl)
    {
        GL = gl;
        SetupMesh();
    }

    /// <summary>
    ///     Allocates the required memory for the mesh and sets up the Vertex Array Object (VAO), Vertex Buffer Object (VBO), and Element Buffer Object (EBO).
    /// </summary>
    public void SetupMesh()
    {
        EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
        VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
        VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);

        VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        VAO.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        VAO.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

        // VAO.VertexAttributePointer(0, VertexData.VerticesSize, VertexAttribPointerType.Float, VertexData.Stride, VertexData.VerticesOffset);
        // VAO.VertexAttributePointer(1, VertexData.NormalsSize, VertexAttribPointerType.Float, VertexData.Stride, VertexData.NormalsOffset);
        // VAO.VertexAttributePointer(2, VertexData.TexCoordsSize, VertexAttribPointerType.Float, VertexData.Stride, VertexData.TexCoordsOffset);
    }

    /// <summary>
    ///     Binds the mesh Vertex Array Object (VAO) to the current OpenGL context.
    /// </summary>
    public void Bind()
        => VAO.Bind();

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
            return;

        if (disposing)
        {
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }

        disposedValue = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
