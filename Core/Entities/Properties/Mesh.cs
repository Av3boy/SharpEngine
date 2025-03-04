using SharpEngine.Core.Attributes;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace Core.Entities.Properties;

/// <summary>
///     Represents a game object mesh.
/// </summary>
public class Mesh
{
    public string Name { get; set; } = "New Mesh";

    /// <summary>
    ///     Gets or sets the mesh vertices.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Vertices { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh normals.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Normals { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] TextureCoordinates { get; set; } = [];
}

/// <summary>
///     Contains extension methods for handling meshes.
/// </summary>
public static class MeshExtensions
{
    /// <summary>
    ///     Gets the vertices of a mesh.
    /// </summary>
    /// <param name="mesh">The mesh who's vertices should be retrieved.</param>
    /// <returns>The vertex data of the mesh (vertex, normal, uv).</returns>
    public static float[] GetVertices(this Mesh mesh)
    {
        var vertices = new List<float>();

        // The current implementation assumes that the mesh has the same amount of vertices, normals and texture coordinates in a specific order.
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
}

/// <summary>
///     Represents a service that loads meshes into the GPU.
/// </summary>
public class MeshService
{
    /// <summary>A global instance of the service.</summary>
    public static readonly MeshService Instance = new();

    private readonly Dictionary<string, Mesh> Meshes = [];

    private MeshService() { }

    /// <summary>
    ///     Loads a mesh into the mesh cache.
    /// </summary>
    /// <param name="identifier">Used to cache the loaded mesh.</param>
    /// <param name="mesh">The mesh that should be stored in to the GPU buffer.</param>
    /// <returns>The loaded mesh.</returns>
    public Mesh LoadMesh(string identifier, Mesh mesh)
    {
        if (Meshes.TryGetValue(identifier, out var cachedMesh))
            return cachedMesh;

        var meshData = mesh.GetVertices();

        var vertexBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);

        unsafe
        {
            fixed (float* meshDataPtr = meshData)
            {
                Window.GL.BufferData(GLEnum.ArrayBuffer, (uint)meshData.Length * sizeof(float), meshDataPtr, GLEnum.StaticDraw);
            }
        }

        Meshes.Add(identifier, mesh);
        return mesh;
    }

    // TODO: Load mesh from file
}