﻿using OpenTK.Mathematics;

namespace Core.Primitives;
public static class Plane
{
    public static GameObject Create(Vector3 position, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
        => new(diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile)
        {
            Position = position,
            Mesh = Mesh
        };

    public static Mesh Mesh { get; } = new()
    {
        Vertices =
        [
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.5f,  0.5f, 0.0f,
             0.5f,  0.5f, 0.0f,
            -0.5f,  0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
        ],
        Normals =
        [
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
        ],
        TextureCoordinates =
        [
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 1.0f,
            0.0f, 1.0f,
            0.0f, 0.0f
        ]
    };
}
