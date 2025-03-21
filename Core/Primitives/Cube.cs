﻿using SharpEngine.Core.Entities.Properties.Meshes;

namespace SharpEngine.Core.Primitives;

/// <summary>
///     Used to create a primitive cube object.
/// </summary>
public static class Cube
{
    /// <summary>The cube mesh.</summary>
    public static readonly Mesh Mesh = new()
    {
        Vertices =
        [
            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,

            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,

             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,

            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,
        ],
        Normals =
        [
              0.0f,  0.0f, -1.0f,
              0.0f,  0.0f, -1.0f,
              0.0f,  0.0f, -1.0f,
              0.0f,  0.0f, -1.0f,
              0.0f,  0.0f, -1.0f,
              0.0f,  0.0f, -1.0f,

              0.0f,  0.0f,  1.0f,
              0.0f,  0.0f,  1.0f,
              0.0f,  0.0f,  1.0f,
              0.0f,  0.0f,  1.0f,
              0.0f,  0.0f,  1.0f,
              0.0f,  0.0f,  1.0f,

             -1.0f,  0.0f,  0.0f,
             -1.0f,  0.0f,  0.0f,
             -1.0f,  0.0f,  0.0f,
             -1.0f,  0.0f,  0.0f,
             -1.0f,  0.0f,  0.0f,
             -1.0f,  0.0f,  0.0f,

              1.0f,  0.0f,  0.0f,
              1.0f,  0.0f,  0.0f,
              1.0f,  0.0f,  0.0f,
              1.0f,  0.0f,  0.0f,
              1.0f,  0.0f,  0.0f,
              1.0f,  0.0f,  0.0f,

              0.0f, -1.0f,  0.0f,
              0.0f, -1.0f,  0.0f,
              0.0f, -1.0f,  0.0f,
              0.0f, -1.0f,  0.0f,
              0.0f, -1.0f,  0.0f,
              0.0f, -1.0f,  0.0f,

              0.0f,  1.0f,  0.0f,
              0.0f,  1.0f,  0.0f,
              0.0f,  1.0f,  0.0f,
              0.0f,  1.0f,  0.0f,
              0.0f,  1.0f,  0.0f,
              0.0f,  1.0f,  0.0f,
        ],
        TextureCoordinates =
        [
              0.0f, 0.0f,
              1.0f, 0.0f,
              1.0f, 1.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,

              0.0f, 0.0f,
              1.0f, 0.0f,
              1.0f, 1.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,

              1.0f, 0.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,
              1.0f, 0.0f,

              1.0f, 0.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,
              1.0f, 0.0f,

              0.0f, 1.0f,
              1.0f, 1.0f,
              1.0f, 0.0f,
              1.0f, 0.0f,
              0.0f, 0.0f,
              0.0f, 1.0f,

              0.0f, 1.0f,
              1.0f, 1.0f,
              1.0f, 0.0f,
              1.0f, 0.0f,
              0.0f, 0.0f,
              0.0f, 1.0f
        ]
    };
}
