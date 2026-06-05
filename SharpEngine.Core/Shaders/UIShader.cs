using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;
using SharpEngine.Core._Resources;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Represents a shader used for rendering UI elements. 
/// </summary>
/// <remarks>
///     This shader is responsible for rendering 2D UI components on the screen, such as buttons, panels, and other interface elements.
///     It is designed to work with the specific vertex and fragment shaders defined for UI rendering in the game engine.
/// </remarks>
internal class UIShader : ShaderBase
{
    /// <summary>
    ///     Ensures that the shader is initialized. 
    /// </summary>
    /// <remarks>
    ///     This method should be called before using the shader to ensure that it is properly loaded and ready for use.
    /// </remarks>
    /// <param name="window">The window where the shader will be used.</param>
    public void EnsureInitialized(Window window)
    {
        if (Shader is not null)
            return;

        Shader = ShaderService.Instance.LoadShader(window, Default.UIVertexShader, Default.UIFragmentShader, nameof(UIShader));
    }

    /// <inheritdoc />
    public override bool SetAttributes(GL gl)
    {
        if (!base.SetAttributes(gl))
            return false;

        gl.EnableVertexAttribArray(0);
        gl.VertexAttribPointer(0, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        gl.EnableVertexAttribArray(1);
        gl.VertexAttribPointer(1, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

        gl.EnableVertexAttribArray(2);
        gl.VertexAttribPointer(2, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);

        return true;
    }
}
