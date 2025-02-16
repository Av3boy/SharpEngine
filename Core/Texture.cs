using StbImageSharp;
using System.IO;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Core
{
    /// <summary>
    ///     Represents a service for loading textures.
    /// </summary>
    public class TextureService
    {
        /// <summary>
        ///     Gets the singleton instance of the <see cref="TextureService"/> class.
        /// </summary>
        public static TextureService Instance { get; } = new TextureService();
        private readonly Dictionary<string, Texture> _textureCache = new();

        // Private constructor to prevent instantiation
        private TextureService() { }

        /// <summary>
        ///     Loads a texture from the specified path.
        /// </summary>
        /// <param name="path">the full path to the texture.</param>
        /// <returns>The loaded texture program.</returns>
        public Texture LoadTexture(string path)
        {
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
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                // Generate a texture
                Window.GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            // Set texture parameters
            Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            Window.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Create a new texture instance and add it to the cache
            var texture = new Texture(handle);
            _textureCache[path] = texture;

            return texture;
        }
    }

    /// <summary>
    ///     Represents a texture program.
    /// </summary>
    public class Texture
    {
        /// <summary>The OpenGL handle for the texture.</summary>
        public readonly uint Handle;

        /// <summary>
        ///     Initializes a new instance of <see cref="Texture"/>.
        /// </summary>
        /// <param name="glHandle"></param>
        public Texture(uint glHandle)
        {
            Handle = glHandle;
        }

        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.

        /// <summary>
        ///     Activate the texture.
        /// </summary>
        /// <param name="unit">The bound texture location.</param>
        public void Use(TextureUnit unit)
        {
            Window.GL.ActiveTexture(unit);
            Window.GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
