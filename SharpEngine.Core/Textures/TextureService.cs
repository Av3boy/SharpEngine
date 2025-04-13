using StbImageSharp;

using System.IO;
using System.Collections.Generic;
using System;

using Silk.NET.OpenGL;
using SharpEngine.Core.Windowing;
using SharpEngine.Core.Components.Properties.Textures;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

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

        if (!File.Exists(path))
            throw new FileNotFoundException($"Texture file not found from path '{path}'.");

        // Check if the texture is already in the cache
        if (_textureCache.TryGetValue(path, out var cachedTexture))
            return cachedTexture;

        // Generate handle
        // TODO: Determine the type of texture.
        var texture = new Texture(Window.GL, path, Silk.NET.Assimp.TextureType.Diffuse);
        texture.Initialize();

        // Add it to the cache
        _textureCache[path] = texture;

        return texture;
    }
}
