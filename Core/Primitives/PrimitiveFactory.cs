using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Properties;

using System;
using System.Numerics;

namespace SharpEngine.Core.Primitives;

/// <summary>
///     Used to create a primitive objects.
/// </summary>
public static class PrimitiveFactory
{
    /// <summary>
    ///     Creates a new <see cref="GameObject"/> by the given primitive type.
    /// </summary>
    /// <param name="primitiveType">The type of primitive to create.</param>
    /// <param name="position">Where the game object should be placed.</param>
    public static GameObject Create(PrimitiveType primitiveType, Vector3 position)
        => Create(primitiveType, position, "DefaultTextures/debug.JPG");

    /// <inheritdoc cref="Create(PrimitiveType, Vector3)"/>
    /// <param name="primitiveType">The type of primitive to be created.</param>
    /// <param name="position">The position where the primitive should be created.</param>
    /// <param name="diffuseMapFile">The diffuse map texture file full path.</param>
    /// <param name="specularMapFile">The specular map texture file full path.</param>
    /// <param name="vertShaderFile">The vertex shader file full path.</param>
    /// <param name="fragShaderFile">The fragment shader file full path.</param>
    /// <returns>A new game object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified primitive type does not exist.</exception>
    public static GameObject Create(PrimitiveType primitiveType, Vector3 position, string diffuseMapFile, string? specularMapFile = null, string? vertShaderFile = null, string? fragShaderFile = null)
        => new(diffuseMapFile, specularMapFile ?? "DefaultTextures/debug.JPG", vertShaderFile ?? "Shaders/shader.vert", fragShaderFile ?? "Shaders/lighting.frag")
        {
            Transform = new Transform { Position = position },
            Mesh = primitiveType switch
            {
                PrimitiveType.Cube => Cube.Mesh,
                PrimitiveType.Plane => Plane.Mesh,
                _ => throw new InvalidOperationException($"A primitive of type {primitiveType} does not exist.")
            }
        };

}
