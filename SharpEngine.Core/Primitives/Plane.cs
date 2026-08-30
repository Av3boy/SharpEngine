using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;
using System;

namespace SharpEngine.Core.Primitives;

/// <summary>
///     Used to create a primitive plane object.
/// </summary>
public static class Plane
{
    /// <summary>The plane mesh.</summary>
    /// <remarks>
    ///     Vertices are stored in interleaved format: position (x, y, z), normal (nx, ny, nz), UV (u, v).
    /// </remarks>
    public static Mesh Mesh { get; } = new(Window.SharedGL,
        vertices:
        [
            // pos.x,  pos.y, pos.z, norm.x, norm.y, norm.z, uv.u,  uv.v
             1f,  1f, 0.0f,  0f, 0f, 1f,  1.0f, 0.0f, // top right
             1f, -1f, 0.0f,  0f, 0f, 1f,  1.0f, 0.0f, // bottom right
            -1f, -1f, 0.0f,  0f, 0f, 1f,  0.0f, 1.0f, // bottom left
            -1f,  1f, 0.0f,  0f, 0f, 1f,  0.0f, 1.0f, // top left
        ],
        indices: [0u, 1u, 3u, 1u, 2u, 3u]);

    internal static Model CreateModel(string diffuseMapFile, string? specularMapFile) => throw new NotImplementedException();
}
