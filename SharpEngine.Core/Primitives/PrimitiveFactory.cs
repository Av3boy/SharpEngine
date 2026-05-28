using SharpEngine.Core.Components.Properties.Meshes;
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
        => Create(primitiveType, position, _Resources.Default.DebugTexture);

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
    {
        Model model = primitiveType switch
        {
            PrimitiveType.Cube => Cube.CreateModel(diffuseMapFile, specularMapFile),
            // PrimitiveType.Plane => [Plane.Mesh],
            _ => throw new InvalidOperationException($"A primitive of type {primitiveType} does not exist.")
        };

        // Shader compilation is context-dependent; the GameObject will acquire the correct shader per-window during rendering.
        return new GameObject()
        {
            Model = model,
            Transform = new Transform((Numerics.Vector3)position),
        };
    }

}
