namespace SharpEngine.Core.Components.Properties.Textures;

/// <summary>
///     Represents the semantic role of a texture in a material.
/// </summary>
public enum TextureType
{
    /// <summary>
    ///     A texture that defines the base color of a material, affecting how it appears under direct lighting.
    /// </summary>
    Diffuse,

    /// <summary>
    ///     The texture that defines the shininess and reflectivity of a material, affecting how it interacts with light and creates highlights.
    /// </summary>
    Specular,

    /// <summary>
    ///     The texture that defines the normal vectors of a surface, which are used to create the illusion of surface detail and depth without adding additional geometry.
    /// </summary>
    Ambient,

    /// <summary>
    ///     The texture that defines the height information of a surface, which can be used to create parallax effects or to simulate surface displacement in advanced rendering techniques.
    /// </summary>
    Height
}