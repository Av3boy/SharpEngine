using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;

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
    public UIShader(GL gl) : base(gl, Defaults.Defaults.UIVertexShader, Defaults.Defaults.UIFragmentShader, nameof(UIShader)) { }

    /// <inheritdoc />
    public override bool SetAttributes(GL gl)
    {
        if (!base.SetAttributes(gl))
            return false;

        // gl.EnableVertexAttribArray(VertexData.VertexIndex);
        // gl.VertexAttribPointer(VertexData.VertexIndex, VertexData.VertexCount, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.VerticesOffset * sizeof(float));
        // 
        // gl.EnableVertexAttribArray(VertexData.NormalIndex);
        // gl.VertexAttribPointer(VertexData.NormalIndex, VertexData.NormalCount, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset * sizeof(float));
        // 
        // gl.EnableVertexAttribArray(VertexData.TexCoordIndex);
        // gl.VertexAttribPointer(VertexData.TexCoordIndex, VertexData.TexCoordCount, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset * sizeof(float));

        return true;
    }
}
