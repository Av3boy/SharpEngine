using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class LampShader : ShaderBase
{
    public LampShader()
    {
        Shader = ShaderService.Instance.LoadShader(PathExtensions.GetAssemblyPath("Shaders/shader.vert"), PathExtensions.GetAssemblyPath("Shaders/shader.frag"), "lamp");

        Vao = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(Vao);

        SetAttributes();
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false;

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
            return false;

        var positionLocationUint = (uint)positionLocation;
        Window.GL.EnableVertexAttribArray(positionLocationUint);
        Window.GL.VertexAttribPointer(positionLocationUint, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        return true;
    }
}
