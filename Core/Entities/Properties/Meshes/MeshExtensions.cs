using System.Collections.Generic;

namespace SharpEngine.Core.Entities.Properties.Meshes;

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

        return [.. vertices];
    }
}
