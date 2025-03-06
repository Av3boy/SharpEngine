using Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties;
using Silk.NET.OpenGL;
using System;

namespace Core.Shaders;

internal class LightingShader : ShaderBase
{
    public LightingShader()
    {
        Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false;

        var positionLocation = (uint)Shader!.GetAttribLocation(ShaderAttributes.Pos);
        Window.GL.EnableVertexAttribArray(positionLocation);
        Window.GL.VertexAttribPointer(positionLocation, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        var normalLocation = (uint)Shader.GetAttribLocation(ShaderAttributes.Normal);
        Window.GL.EnableVertexAttribArray(normalLocation);
        Window.GL.VertexAttribPointer(normalLocation, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

        var texCoordLocation = (uint)Shader.GetAttribLocation(ShaderAttributes.TexCoords);
        Window.GL.EnableVertexAttribArray(texCoordLocation);
        Window.GL.VertexAttribPointer(texCoordLocation, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);

        return true;
    }
}
