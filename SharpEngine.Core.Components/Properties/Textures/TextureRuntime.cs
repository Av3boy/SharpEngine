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

        // Flip the rows because System.Drawing bitmaps are top-left origin while OpenGL expects bottom-left.
        var flipped = new byte[rgbaData.Length];
        int rowBytes = width * 4;
        for (int y = 0; y < height; y++)
        {
            int srcRow = y * rowBytes;
            int dstRow = (height - 1 - y) * rowBytes;
            System.Buffer.BlockCopy(rgbaData, srcRow, flipped, dstRow, rowBytes);
        }

        // Upload the pixel data (flipped vertically)
        _gl.TexImage2D<byte>(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, flipped);

        SetParameters();
        _gl.GenerateMipmap(GLEnum.Texture2D);
    }
}
