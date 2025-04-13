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

    public void SetParameters()
    {
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    // Multiple textures can be bound, if your shader needs more than just one.
    // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
    // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.

    /// <summary>
    ///     Activate the texture.
    /// </summary>
    /// <param name="unit">The bound texture location.</param>
    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(Handle);
    }
}
