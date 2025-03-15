using StbImageSharp;

using System.IO;
using System.Collections.Generic;
using System;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Textures;

/// <summary>
///     Represents a service for loading textures.
/// </summary>
public class TextureService
{
    /// <summary>
    ///     Gets the singleton instance of <see cref="TextureService"/>.
    /// </summary>
    public static TextureService Instance { get; } = new TextureService();
    private readonly Dictionary<string, Texture> _textureCache = [];

    // Private constructor to prevent instantiation
    private TextureService() { }

    /// <summary>
    ///     Loads a texture from the specified path.
    /// </summary>
    /// <param name="path">the full path to the texture.</param>
    /// <returns>The loaded texture program.</returns>
    public Texture LoadTexture(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new FileNotFoundException("No texture file provided.");

        // Check if the texture is already in the cache
        if (_textureCache.TryGetValue(path, out var cachedTexture))
            return cachedTexture;

        // Generate handle
        uint handle = Window.GL.GenTexture();

        // Bind the handle
        Window.GL.ActiveTexture(TextureUnit.Texture0);
        Window.GL.BindTexture(TextureTarget.Texture2D, handle);

        // OpenGL has its texture origin in the lower left corner instead of the top left corner,
        // so we tell StbImageSharp to flip the image when loading.
        StbImage.stbi_set_flip_vertically_on_load(1);

        // Here we open a stream to the file and pass it to StbImageSharp to load.
        using (Stream stream = File.OpenRead(path))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            // Generate a texture
            Window.GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data.AsSpan());
        }

        // Set texture parameters
        Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // Generate mipmaps
        Window.GL.GenerateMipmap(GLEnum.Texture2D);

        // Create a new texture instance and add it to the cache
        var texture = new Texture(handle);
        _textureCache[path] = texture;

        return texture;
    }
}
