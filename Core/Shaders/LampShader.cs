using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Extensions;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class LampShader : ShaderBase
{
    public LampShader()
    {
        Shader = ShaderService.Instance.LoadShader(PathExtensions.GetPath("Shaders/shader.vert"), PathExtensions.GetPath("Shaders/shader.frag"), "lamp");

        Vao = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(Vao);

        SetAttributes();
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
