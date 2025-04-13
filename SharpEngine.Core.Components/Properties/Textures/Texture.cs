using Silk.NET.Assimp;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Components.Properties.Textures;

/// <summary>
///     Represents a texture program.
/// </summary>
public partial class Texture
{
    /// <summary>The OpenGL handle for the texture.</summary>
    public readonly uint Handle;

    public string Path { get; set; }
    
    public TextureType Type { get; }

    private readonly GL _gl;

    /// <summary>
    ///     Initializes a new instance of <see cref="Texture"/>.
    /// </summary>
    /// <param name="glHandle">The handle to the texture.</param>
    public Texture(uint glHandle, GL gl)
    {
        Handle = glHandle;
        _gl = gl;
    }
}
