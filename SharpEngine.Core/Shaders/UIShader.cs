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
internal class UIShader : Shader
{
    public UIShader(GL gl) : base(gl, Default.UIVertexShader, Default.UIFragmentShader, nameof(UIShader)) { }

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
