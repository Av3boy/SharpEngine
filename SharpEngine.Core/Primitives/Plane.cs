using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;

namespace SharpEngine.Core.Primitives;

/// <summary>
///     Used to create a primitive plane object.
/// </summary>
public static class Plane
{
    /// <summary>The plane mesh.</summary>
    public static Mesh Mesh { get; } = new(Window.GL)
    {
        Vertices =
        [
             1f,  1f, 0.0f, // top right
             1f, -1f, 0.0f, // bottom right
            -1f, -1f, 0.0f, // bottom left
            -1f,  1f, 0.0f, // top left
        ],
        Normals =
        [
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
        ],
        TextureCoordinates =
        [
            1.0f, 0.0f,
            1.0f, 0.0f,
            0.0f, 1.0f,
            0.0f, 1.0f,
        ],
        Indices =
        [
           0, 1, 3, // first triangle
           1, 2, 3, // second triangle
        ]
    };
}
