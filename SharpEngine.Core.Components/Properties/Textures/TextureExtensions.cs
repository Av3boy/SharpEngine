using StbImageSharp;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Components.Properties.Textures;

public partial class Texture
{
    private readonly Dictionary<string, Texture> _textureCache = [];

    /// <summary>
    ///     Loads a texture from the specified path.
    /// </summary>
    /// <param name="path">the full path to the texture.</param>
    /// <returns>The loaded texture program.</returns>
    public Texture LoadTexture(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new FileNotFoundException("No texture file provided.");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Texture file not found from path '{path}'.");

        // Check if the texture is already in the cache
        if (_textureCache.TryGetValue(path, out var cachedTexture))
            return cachedTexture;

        // Generate texture
        var texture = new Texture(_gl, path, Silk.NET.Assimp.TextureType.Diffuse);

        // Bind the handle
        texture.Use();

        // OpenGL has its texture origin in the lower left corner instead of the top left corner,
        // so we tell StbImageSharp to flip the image when loading.
        StbImage.stbi_set_flip_vertically_on_load(1);

        // Here we open a stream to the file and pass it to StbImageSharp to load.
        using (Stream stream = File.OpenRead(path))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            // Generate a texture
            _gl.TexImage2D<byte>(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data.AsSpan());
        }

        // Set texture parameters
        texture.SetParameters();

        // Generate mipmaps
        _gl.GenerateMipmap(GLEnum.Texture2D);

        // Add it to the cache
        _textureCache[path] = texture;

        return texture;
    }

}
