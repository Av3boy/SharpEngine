﻿using Core.Shaders;
using System.Numerics;

namespace Core.Entities.Properties;

/// <summary>
///     Represents the material rendered onto a game object.
/// </summary>
public class Material
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Material"/>.
    /// </summary>
    public Material()
    {
        Specular = new(0.5f, 0.5f, 0.5f);
        Shininess = 32.0f;
    }

    /// <summary>
    ///    Gets or sets the diffuse map texture.
    /// </summary>
    public Texture DiffuseMap { get; set; }

    /// <summary>
    ///   Gets or sets the specular map texture.
    /// </summary>
    public Texture SpecularMap { get; set; }

    /// <summary>
    ///    Gets or sets the shader used to render the material.
    /// </summary>
    public Shader Shader { get; set; }

    internal readonly int diffuseUnit = 0;
    internal readonly int specularUnit = 1;

    /// <summary>
    ///    Gets or sets the ambient color of the material.
    /// </summary>
    public Vector3 Specular { get; set; }

    /// <summary>
    ///   Gets or sets the shininess of the material.
    /// </summary>
    public float Shininess { get; set; }
}
