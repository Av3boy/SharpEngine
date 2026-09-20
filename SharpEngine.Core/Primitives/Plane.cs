using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Entities.Properties.Meshes;
using Silk.NET.OpenGL;
using System;

namespace SharpEngine.Core.Primitives;

/// <summary>
///     Used to create a primitive plane object.
/// </summary>
public static class Plane
{
    /// <summary>Interleaved vertex data for the plane (position XYZ, normal XYZ, UV UV).</summary>
    public static readonly float[] Vertices =
    [
        // pos.x,  pos.y, pos.z, norm.x, norm.y, norm.z, uv.u,  uv.v
         1f,  1f, 0.0f,  0f, 0f, 1f,  1.0f, 0.0f, // top right
         1f, -1f, 0.0f,  0f, 0f, 1f,  1.0f, 0.0f, // bottom right
        -1f, -1f, 0.0f,  0f, 0f, 1f,  0.0f, 1.0f, // bottom left
        -1f,  1f, 0.0f,  0f, 0f, 1f,  0.0f, 1.0f, // top left
    ];

    /// <summary>Index buffer for the plane.</summary>
    public static readonly uint[] Indices = [0u, 1u, 3u, 1u, 2u, 3u];

    /// <summary>
    ///     Creates a GL-bound Mesh instance for the provided GL context.
    ///     This factory avoids creating a single global Mesh bound to SharedGL.
    /// </summary>
    public static Mesh CreateMesh(GL gl) => new Mesh(gl, Vertices, Indices);

    internal static Model CreateModel(GL gl, string diffuseMapFile, string? specularMapFile) => throw new NotImplementedException();
}
