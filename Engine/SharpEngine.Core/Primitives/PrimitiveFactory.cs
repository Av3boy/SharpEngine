using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Numerics;
using Tutorial;

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
        // var material = diffuseMapFile, specularMapFile, 

        Model_Old model = primitiveType switch
        {
            PrimitiveType.Cube => Cube.Model,
            // PrimitiveType.Plane => [Plane.Mesh],
            _ => throw new InvalidOperationException($"A primitive of type {primitiveType} does not exist.")
        };

        var shader = ShaderService.Instance.LoadShader(vertShaderFile ?? _Resources.Default.VertexShader, fragShaderFile ?? _Resources.Default.FragmentShader, "lighting");
        return new GameObject(shader, model)
        {
            Transform = new Transform((Numerics.Vector3)position),
        };
    }

}
