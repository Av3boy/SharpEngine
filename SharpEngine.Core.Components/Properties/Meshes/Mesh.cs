using ObjLoader.Loader.Data.Elements;
using SharpEngine.Core.Attributes;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using Silk.NET.OpenGL;
using Tutorial;

namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Represents a game object mesh.
/// </summary>
public class Mesh
{
    /// <summary>Gets or sets a identifying name for the mesh.</summary>
    public string Name { get; set; } = "New Mesh";

    /// <summary>
    ///     Gets or sets the mesh vertices.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Vertices { get; set; } = [];
    public List<Vertex> Vertices2 { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh normals.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Normals { get; set; } = [];
    public List<Normal> Normals2 { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] TextureCoordinates { get; set; } = [];

    public List<TextureCoordinate> TextureCoordinates2 { get; set; } = [];

    /// <summary>
    ///     Gets or sets the indicies of the mesh.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public uint[] Indices { get; set; } = [];

    public List<Group> Groups { get; set; } = [];
    public List<Material> Materials { get; set; }= [];

    // --------------------------------------------------

    public Mesh(GL gl, float[] vertices, uint[] indices) //, List<Texture> textures)
    {
        GL = gl;
        Vertices = vertices;
        Indices = indices;
        //Textures = textures;
        SetupMesh();
    }

    public Mesh()
    {

    }

    public VertexArrayObject<float, uint> VAO { get; set; }
    public BufferObject<float> VBO { get; set; }
    public BufferObject<uint> EBO { get; set; }
    public GL GL { get; }


    public unsafe void SetupMesh()
    {
        // EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
        // VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
        // VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);
        VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
        VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
    }

    public void Bind()
    {
        VAO.Bind();
    }

    public void Dispose()
    {
        // Textures = null;
        VAO.Dispose();
        VBO.Dispose();
        EBO.Dispose();
    }
}