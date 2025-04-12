using System;
using System.IO;
using System.Collections.Generic;
using SharpEngine.Core.Windowing;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Contains all the shaders used in the game.
/// </summary>
public class ShaderService
{
    /// <summary>
    ///     Gets the singleton instance of the <see cref="ShaderService"/>.
    /// </summary>
    public static ShaderService Instance { get; } = new ShaderService();

    // TODO: Cache data is never set
    private readonly Dictionary<string, Shader> _shaderCache = [];

    /// <summary>
    ///    Gets or sets whether there are shaders to load.
    /// </summary>
    public bool HasShadersToLoad { get; set; } = true;

    /// <summary>
    ///     Private constructor to prevent instantiation.
    /// </summary>
    private ShaderService() { }

    /// <summary>
    ///     Gets all the shaders in the cache.
    /// </summary>
    /// <returns>All the shaders found from the cache.</returns>
    public List<Shader> GetAll()
    {
        HasShadersToLoad = false;
        return [.. _shaderCache.Values];
    }

    /// <summary>
    ///     Gets a shader by its name.
    /// </summary>
    /// <param name="name">The name of the shader to be found.</param>
    /// <returns>The found shader.</returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown if a shader by that is not found.
    ///     This exception is thrown to make sure there are no unexpected issues made by the developer.
    /// </exception>
    public Shader GetByName(string name)
    {
        if (_shaderCache.TryGetValue(name, out var cachedShader))
            return cachedShader;

        throw new KeyNotFoundException($"Shader with name {name} not found in cache.");
    }

    /// <summary>
    ///     Loads a shader from the specified vertex and fragment paths. <br />
    ///     If the shader is loaded already, adds it to the cache.
    /// </summary>
    /// <param name="vertPath">The vertex shader full path.</param>
    /// <param name="fragPath">The fragment shader full path.</param>
    /// <param name="name">A name identifier for the shader.</param>
    /// <returns>A shader with the given name.</returns>
    /// <exception cref="FileNotFoundException">Thrown when either the vertex or fragment shader is not found.</exception>
    public Shader LoadShader(string vertPath, string fragPath, string name)
    {
        // Check if the shader is already in the cache
        if (_shaderCache.TryGetValue(name, out var cachedShader))
            return cachedShader;

        if (!File.Exists(vertPath))
        {
            Console.WriteLine($"Vertex shader file not found: {vertPath}");
            throw new FileNotFoundException($"Vertex shader file not found: {vertPath}");
        }

        if (!File.Exists(fragPath))
        {
            Console.WriteLine($"Fragment shader file not found: {fragPath}");
            throw new FileNotFoundException($"Fragment shader file not found: {fragPath}");
        }

        // Create a new shader instance and add it to the cache
        var shader = new Shader(Window.GL, vertPath, fragPath, name).Initialize();
        _shaderCache[name] = shader;

        HasShadersToLoad = true;

        return shader;
    }
}
