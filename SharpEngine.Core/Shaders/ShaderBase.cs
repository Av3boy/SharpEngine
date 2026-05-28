using SharpEngine.Core.Components.Properties;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using System;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Represents the base class for all shaders.
/// </summary>
public abstract class ShaderBase
{
    private static readonly ILogger<ShaderBase> Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ShaderBase>();

    /// <summary>Gets the shader.</summary>
    public Shader? Shader { get; protected set; }

    /// <summary>Gets or sets the vertex array object.</summary>
    public uint Vao { get; set; }

    /// <summary>
    ///    Sets the attributes for the shader.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the attributes were set successfully; otherwise <see langword="false" />.
    /// </returns>
    public virtual bool SetAttributes(GL gl)
    {
        if (Shader is null)
        {
            Logger.LogError("Unable to set shader attributes, shader not found.");
            return false;
        }

        return true;
    }
}
