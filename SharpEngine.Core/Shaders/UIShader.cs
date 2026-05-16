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

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
            return false;

        var positionLocationUint = (uint)positionLocation;
        gl.EnableVertexAttribArray(positionLocationUint);
        gl.VertexAttribPointer(positionLocationUint, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Normal, out int normalLocation))
            return false;

        var normalLocationUint = (uint)normalLocation;
        gl.EnableVertexAttribArray(normalLocationUint);
        gl.VertexAttribPointer(normalLocationUint, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.TexCoords, out int texCoordLocation))
            return false;

        var texCoordLocationUint = (uint)texCoordLocation;
        gl.EnableVertexAttribArray(texCoordLocationUint);
        gl.VertexAttribPointer(texCoordLocationUint, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);

        return true;
    }
}
