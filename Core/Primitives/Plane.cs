using OpenTK.Mathematics;

namespace Core.Primitives;

/// <summary>
///     Used to create a primitive plane object.
///     TODO: Primitive base class that forces the Mesh and Create to be implemented.
/// </summary>
public static class Plane
{
    /// <summary>
    ///     Creates a new <see cref="GameObject"/> with a plane mesh.
    /// </summary>
    /// <param name="position">Where the game object should be placed.</param>
    /// <param name="diffuseMapFile">The diffuse map texture file full path.</param>
    /// <param name="specularMapFile">The specular map texture file full path.</param>
    /// <param name="vertShaderFile">The vertex shader file full path.</param>
    /// <param name="fragShaderFile">The fragment shader file full path.</param>
    /// <returns>A new plane gameo bject.</returns>
    public static GameObject Create(Vector3 position, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
        => new(diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile)
        {
            Position = position,
            Mesh = Mesh
        };

    /// <summary>The plane mesh.</summary>

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
