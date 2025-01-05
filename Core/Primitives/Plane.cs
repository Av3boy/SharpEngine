﻿using Core.Entities.Properties;

namespace Core.Primitives;

/// <summary>
///     Used to create a primitive plane object.
/// </summary>
public static class Plane
{
    /// <summary>The plane mesh.</summary>
    public static Mesh Mesh { get; } = new()
    {
        Vertices =
        [
             0.5f,  0.5f, 0.0f, // top right
             0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, // top left
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
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f,
        ]
    };
}
