using Silk.NET.Assimp;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Components.Properties.Textures;

/// <summary>
///     Represents a texture program.
/// </summary>
public partial class Texture : IDisposable
{
    /// <summary>The OpenGL handle for the texture.</summary>
    public readonly uint Handle;
    public readonly TextureType Type;
    public readonly string Path;
    
    private readonly GL _gl;

    /// <summary>
    ///     Initializes a new instance of <see cref="Texture"/>.
    /// </summary>
    public Texture(GL gl, string path, TextureType type = TextureType.Diffuse)
    {
        _gl = gl;
        Handle = _gl.GenTexture();

        Path = path;
        Type = type;

        Initialize();
    }
}
