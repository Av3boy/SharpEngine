using Silk.NET.OpenGL;
using System.IO;

namespace SharpEngine.Core.Components.Properties.Textures;

public partial class Texture
{
    /// <summary>
    /// Creates a texture from raw RGBA bytes.
    /// </summary>
    public Texture(GL gl, byte[] rgbaData, int width, int height, string path = "<runtime>", TextureType type = TextureType.Diffuse)
    {
        _gl = gl;
        Path = path;
        Type = type;
        Handle = _gl.GenTexture();

        Use();

        // Upload the pixel data
        _gl.TexImage2D<byte>(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, rgbaData);

        SetParameters();
        _gl.GenerateMipmap(GLEnum.Texture2D);
    }
}
