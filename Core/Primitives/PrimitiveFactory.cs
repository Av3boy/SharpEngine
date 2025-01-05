using Core.Entities;
using Core.Entities.Properties;

using OpenTK.Mathematics;
using System;

namespace Core.Primitives;
public static class PrimitiveFactory
{
    /// <summary>
    ///     Creates a new <see cref="GameObject"/> by the given primitive type.
    /// </summary>
    /// <param name="primitiveType"></param>
    /// <param name="position">Where the game object should be placed.</param>
    /// <param name="diffuseMapFile">The diffuse map texture file full path.</param>
    /// <param name="specularMapFile">The specular map texture file full path.</param>
    /// <param name="vertShaderFile">The vertex shader file full path.</param>
    /// <param name="fragShaderFile">The fragment shader file full path.</param>
    /// <returns>A new gameobject.</returns>
    public static GameObject Create(PrimitiveType primitiveType, Vector3 position, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        var mesh = primitiveType switch
        {
            PrimitiveType.Cube => Cube.Mesh,
            PrimitiveType.Plane => Plane.Mesh,
            _ => throw new InvalidOperationException($"A primitive of type {primitiveType} does not exist.")
        };

        return new(diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile)
        {
            Transform = new Transform { Position = position },
            Mesh = mesh
        };
    }
}
