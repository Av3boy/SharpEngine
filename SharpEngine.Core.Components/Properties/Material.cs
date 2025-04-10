using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Textures;
using System.Numerics;

namespace SharpEngine.Core.Components.Properties;

/// <summary>
///     Represents the material rendered onto a game object.
/// </summary>
public class Material
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Material"/>.
    /// </summary>
    public Material(Shader shader, Texture diffuseMap, Texture? specularMap = null)
    {
        Shader = shader;
        DiffuseMap = diffuseMap;
        SpecularMap = specularMap ?? diffuseMap;

        Specular = new(0.5f, 0.5f, 0.5f);
        Shininess = 32.0f;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Material"/>.
    /// </summary>
    /// <param name="materialName">The name assigned to the new material.</param>
    public Material(string materialName)
    {
        Name = materialName;
    }

    public string Name { get; set; }

    /// <summary>
    ///    Gets or sets the diffuse map texture.
    /// </summary>
    public Texture DiffuseMap { get; set; }

    /// <summary>
    ///   Gets or sets the specular map texture.
    /// </summary>
    public Texture SpecularMap { get; set; }

    /// <summary>Gets whether the material uses a specular map.</summary>
    public bool UseSpecularMap => SpecularMap.Handle != DiffuseMap.Handle;

    /// <summary>
    ///    Gets or sets the shader used to render the material.
    /// </summary>
    public Shader Shader { get; set; }

    public const int DIFFUSE_UNIT = 0;
    public const int SPECULAR_UNIT = 1;

    /// <summary>
    ///    Gets or sets the ambient color of the material.
    /// </summary>
    public Vector3 Specular { get; set; }

    /// <summary>
    ///   Gets or sets the shininess of the material.
    /// </summary>
    public float Shininess { get; set; }

    public Vector3 AmbientColor { get; set; }
    public Vector3 DiffuseColor { get; set; }
    public Vector3 SpecularColor { get; set; }
    public float SpecularCoefficient { get; set; }

    public float Transparency { get; set; }

    public int IlluminationModel { get; set; }

    public string AmbientTextureMap { get; set; }
    public string DiffuseTextureMap { get; set; }

    public string SpecularTextureMap { get; set; }
    public string SpecularHighlightTextureMap { get; set; }

    public string BumpMap { get; set; }
    public string DisplacementMap { get; set; }
    public string StencilDecalMap { get; set; }

    public string AlphaTextureMap { get; set; }
}
