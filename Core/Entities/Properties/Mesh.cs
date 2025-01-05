﻿using System.Collections.Generic;

namespace Core.Entities.Properties;

/// <summary>
///     Represents a game object mesh.
/// </summary>
public class Mesh
{
    /// <summary>
    ///     Gets or sets the mesh vertices.
    /// </summary>
    public float[] Vertices { get; set; }

    /// <summary>
    ///     Gets or sets the mesh normals.
    /// </summary>
    public float[] Normals { get; set; }

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates.
    /// </summary>
    public float[] TextureCoordinates { get; set; }
}

public static class MeshExtensions
{
    public static float[] GetVertices(this Mesh mesh)
    {
        var vertices = new List<float>();

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