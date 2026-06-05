using SharpEngine.Core.Extensions;

namespace SharpEngine.Core._Resources;

/// <summary>
///     Contains default resources used by the engine.
/// </summary>
public static class Default
{
    /// <summary>Gets the path to the debug texture.</summary>
    public static string DebugTexture => PathExtensions.GetAssemblyPath("_Resources\\Textures\\debug.JPG");

    /// <summary>Gets the path to the default vertex shader.</summary>
    public static string VertexShader => PathExtensions.GetAssemblyPath("_Resources\\Shaders\\shader.vert");

    /// <summary>Gets the path to the lighting fragment shader.</summary>
    public static string FragmentShader => PathExtensions.GetAssemblyPath("_Resources\\Shaders\\lighting.frag");

    /// <summary>Gets the path to the default shader used for rendering light sources.</summary>
    public static string LightShader => PathExtensions.GetAssemblyPath("_Resources\\Shaders\\shader.frag");

    /// <summary>Gets the path to the default vertex shader used for rendering UI elements.</summary>
    public static string UIVertexShader => PathExtensions.GetAssemblyPath("_Resources\\Shaders\\uiShader.vert");

    /// <summary>Gets the path to the default fragment shader used for rendering UI elements.</summary>
    public static string UIFragmentShader => PathExtensions.GetAssemblyPath("_Resources\\Shaders\\uiShader.frag");
}
