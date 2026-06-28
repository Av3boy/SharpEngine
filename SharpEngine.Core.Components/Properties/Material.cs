using Silk.NET.OpenGL;
using System.Numerics;
using Shader = SharpEngine.Core.Shaders.Shader;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

namespace SharpEngine.Core.Components.Properties;

public record TextureDto
{
    public TextureDto(string path, Texture? texture = null)
    {
        Path = path;
        Texture = texture;
    }

    public string Path { get; set; }
    public Texture? Texture { get; set; }
}

public enum TextureUnitIndex : int
{
    /// <summary>The texture unit for the diffuse map.</summary>
    DIFFUSE_UNIT = 0,

    /// <summary>The texture unit for the specular map.</summary>
    SPECULAR_UNIT = 1
}

/// <summary>
///     Represents the material rendered onto a game object.
/// </summary>
public class Material
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Material"/>.
    /// </summary>
    /// <param name="materialName">The name assigned to the new material.</param>
    public Material(string materialName)
        : this(materialName, diffuseMap: null, specularMap: null) { }

    /// <summary>
    ///     Initializes a new instance of <see cref="Material"/>.
    /// </summary>
    /// <param name="materialName">The name assigned to the new material.</param>
    /// <param name="diffuseMap">The diffuse map texture of the material.</param>
    /// <param name="specularMap">The specular map texture of the material. Defaults to the diffuse map if not provided.</param>
    public Material(string materialName, Texture? diffuseMap = null, Texture? specularMap = null)
    {
        DiffuseMap = new TextureDto(diffuseMap?.Path ?? string.Empty, diffuseMap);
        SpecularMap = new TextureDto(specularMap?.Path ?? string.Empty, specularMap);

        Specular = new(0.5f, 0.5f, 0.5f);
        Shininess = 32.0f;

        Name = materialName;
    }

    // Resolve dependency issue with GL to fix this.
    // public Shader Shader { get; init; }
    public Shader Shader { get; set; }

    /// <summary>Gets or sets the name of the material.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets the diffuse map texture.</summary>
    public TextureDto? DiffuseMap { get; set; }

    /// <summary>Gets or sets the specular map texture.</summary>
    public TextureDto? SpecularMap { get; set; }

    /// <summary>Gets or sets the specular color of the material.</summary>
    public Vector3 Specular { get; set; }

    /// <summary>Gets or sets the shininess of the material.</summary>
    public float Shininess { get; set; }

    /// <summary>Gets or sets the ambient color of the material.</summary>
    public Vector3 AmbientColor { get; set; }

    /// <summary>Gets or sets the diffuse color of the material.</summary>
    public Vector3 DiffuseColor { get; set; }

    /// <summary>Gets or sets the specular color of the material.</summary>
    public Vector3 SpecularColor { get; set; }

    /// <summary>Gets or sets the specular coefficient of the material.</summary>
    public float SpecularCoefficient { get; set; }

    /// <summary>Gets or sets the transparency of the material.</summary>
    public float Transparency { get; set; }

    /// <summary>Gets or sets the illumination model of the material.</summary>
    public int IlluminationModel { get; set; }

    /// <summary>Gets or sets the path to the ambient texture map.</summary>
    public TextureDto? AmbientTextureMap { get; set; }

    /// <summary>Gets or sets the path to the specular highlight texture map.</summary>
    public TextureDto? SpecularHighlightTextureMap { get; set; }

    /// <summary>Gets or sets the path to the bump map.</summary>
    public TextureDto? BumpMap { get; set; }

    /// <summary>Gets or sets the path to the displacement map.</summary>
    public TextureDto? DisplacementMap { get; set; }

    /// <summary>Gets or sets the path to the stencil decal map.</summary>
    public TextureDto? StencilDecalMap { get; set; }

    /// <summary>Gets or sets the path to the alpha texture map.</summary>
    public TextureDto? AlphaTextureMap { get; set; }

    /// <summary>
    ///     Sets the uniform values for the material in the specified shader.
    /// </summary>
    /// <param name="shader">The shader to set the uniform values in.</param>
    public void SetUniformValues(Shader shader)
    {
        // TODO: Get all shader uniforms and set their values automatically

        if (DiffuseMap is not null)
        {
            DiffuseMap.Texture?.Use(TextureUnit.Texture0);
            shader.SetTextureUnit("material.diffuse", TextureUnitIndex.DIFFUSE_UNIT);
        }

        if (SpecularMap is not null)
        {
            SpecularMap.Texture?.Use(TextureUnit.Texture1);
            shader.SetTextureUnit("material.specular", TextureUnitIndex.SPECULAR_UNIT);
        }
        else
            shader.SetTextureUnit("material.specular", TextureUnitIndex.DIFFUSE_UNIT);

        shader.SetFloat("material.shininess", Shininess);

    }
}
