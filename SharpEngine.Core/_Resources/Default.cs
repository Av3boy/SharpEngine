using SharpEngine.Core.Extensions;

namespace SharpEngine.Core._Resources;

/// <summary>
///     Contains default resources used by the engine.
/// </summary>
public struct Default
{
    /// <summary>Gets the path to the debug texture.</summary>
    public static string DebugTexture => PathExtensions.GetAssemblyPath("Textures\\DefaultTextures\\debug.JPG");

    /// <summary>Gets the path to the default vertex shader.</summary>
    public static string VertexShader => PathExtensions.GetAssemblyPath("Shaders\\shader.vert");

    /// <summary>Gets the path to the lighting fragment shader.</summary>
    public static string FragmentShader => PathExtensions.GetAssemblyPath("Shaders\\lighting.frag");
}
