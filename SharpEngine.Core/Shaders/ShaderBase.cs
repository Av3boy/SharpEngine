using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Represents the base class for all shaders.
/// </summary>
public abstract class ShaderBase
{
    private static readonly ILogger<ShaderBase> Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ShaderBase>();

    /// <summary>Gets the shader.</summary>
    public Shader? Shader { get; protected set; }


}
