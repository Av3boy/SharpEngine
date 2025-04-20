using Silk.NET.OpenGL;
using System.Numerics;
using Shader = SharpEngine.Core.Shaders.Shader;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

namespace SharpEngine.Core.Components.Properties;

/// <summary>
///     Represents the material rendered onto a game object.
/// </summary>
public class Material
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Material"/>.
    /// </summary>
    /// <param name="shader">The shader used to render the material.</param>
    /// <param name="diffuseMap">The diffuse map texture of the material.</param>
    /// <param name="specularMap">The specular map texture of the material. Defaults to the diffuse map if not provided.</param>
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

    /// <summary>Gets or sets the name of the material.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets the diffuse map texture.</summary>
    public Texture DiffuseMap { get; set; }

    /// <summary>Gets or sets the path to the diffuse texture map.</summary>
    public string DiffuseTextureMap { get; set; }

    /// <summary>Gets or sets the specular map texture.</summary>
    public Texture SpecularMap { get; set; }

    /// <summary>Gets or sets the path to the specular texture map.</summary>
    public string SpecularTextureMap { get; set; }

    /// <summary>Gets a value indicating whether the material uses a specular map.</summary>
    public bool UseSpecularMap => SpecularMap.Handle != DiffuseMap.Handle;

    /// <summary>Gets or sets the shader used to render the material.</summary>
    public Shader Shader { get; set; }

    /// <summary>The texture unit for the diffuse map.</summary>
    public const int DIFFUSE_UNIT = 0;

    /// <summary>The texture unit for the specular map.</summary>
    public const int SPECULAR_UNIT = 1;

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
    public string AmbientTextureMap { get; set; }

    /// <summary>Gets or sets the path to the specular highlight texture map.</summary>
    public string SpecularHighlightTextureMap { get; set; }

    /// <summary>Gets or sets the path to the bump map.</summary>
    public string BumpMap { get; set; }

    /// <summary>Gets or sets the path to the displacement map.</summary>
    public string DisplacementMap { get; set; }

    /// <summary>Gets or sets the path to the stencil decal map.</summary>
    public string StencilDecalMap { get; set; }

    /// <summary>Gets or sets the path to the alpha texture map.</summary>
    public string AlphaTextureMap { get; set; }

    /// <summary>
    ///     Sets the uniform values for the material in the specified shader.
    /// </summary>
    /// <param name="shader">The shader to set the uniform values in.</param>
    public void SetUniformValues(Shader shader)
    {
        // TODO: Get all shader uniforms and set their values automatically

        DiffuseMap.Use(TextureUnit.Texture0);
        shader.SetInt("material.diffuse", DIFFUSE_UNIT);

        if (UseSpecularMap)
        {
            SpecularMap.Use(TextureUnit.Texture1);
            shader.SetInt("material.specular", SPECULAR_UNIT);
            shader.SetVector3("material.specular", Specular);
            shader.SetFloat("material.shininess", Shininess);
        }
        else
        {
            shader.SetInt("material.specular", 0);
            shader.SetVector3("material.specular", Vector3.Zero);
            shader.SetFloat("material.shininess", 0);
        }
    }
}
