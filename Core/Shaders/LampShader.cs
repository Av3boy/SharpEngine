using Core.Entities.Properties;
using Silk.NET.OpenGL;
using System;

namespace Core.Shaders;

internal class LampShader : ShaderBase
{
    public LampShader()
    {
        Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag", "lamp");
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false;

        var positionLocation = (uint)Shader!.GetAttribLocation(ShaderAttributes.Pos);
        Window.GL.EnableVertexAttribArray(positionLocation);
        Window.GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        return true;
    }
}
