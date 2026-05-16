using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Contains all the shaders used in the game.
/// </summary>
public class ShaderService
{
    private static readonly ILogger<ShaderService> Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ShaderService>();

    /// <summary>
    ///     Gets the singleton instance of the <see cref="ShaderService"/>.
    /// </summary>
    public static ShaderService Instance { get; } = new ShaderService();

    private readonly Dictionary<ShaderCacheKey, Shader> _shaderCache = [];

    private readonly record struct ShaderCacheKey(string Name, object ShareGroupKey);

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
        // Legacy API: return first shader found with this name.
        foreach (var kvp in _shaderCache)
            if (kvp.Key.Name == name)
                return kvp.Value;

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
    public Shader LoadShader(Window window, string vertPath, string fragPath, string name)
        => LoadShader(window.GetGL(), GetShareGroupKey(window), vertPath, fragPath, name);

    public Shader LoadShader(GL gl, string vertPath, string fragPath, string name)
        => LoadShader(gl, shareGroupKey: gl, vertPath, fragPath, name);

    public Shader LoadShader(GL gl, object shareGroupKey, string vertPath, string fragPath, string name)
    {
        var cacheKey = new ShaderCacheKey(name, shareGroupKey);

        // Check if the shader is already in the cache for this share group.
        if (_shaderCache.TryGetValue(cacheKey, out var cachedShader))
            return cachedShader;

        if (!File.Exists(vertPath))
        {
            Logger.LogInformation("Vertex shader file not found: {VertPath}", vertPath);
            throw new FileNotFoundException($"Vertex shader file not found: {vertPath}");
        }

        if (!File.Exists(fragPath))
        {
            Logger.LogInformation("Fragment shader file not found: {FragPath}", fragPath);
            throw new FileNotFoundException($"Fragment shader file not found: {fragPath}");
        }

        // Create a new shader instance and add it to the cache.
        // Shader program objects are shareable across contexts *only* when those contexts share.
        var shader = new Shader(gl, vertPath, fragPath, name).Initialize();
        _shaderCache[cacheKey] = shader;

        HasShadersToLoad = true;

        return shader;
    }

    private static object GetShareGroupKey(Window window)
        => (object?)window.SharedContext ?? (object?)window.GLContext ?? (object)window;
}
