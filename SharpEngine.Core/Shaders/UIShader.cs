using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;
using SharpEngine.Core._Resources;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class UIShader : ShaderBase
{
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
