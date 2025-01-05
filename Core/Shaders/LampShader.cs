using Core.Entities.Properties;
using OpenTK.Graphics.OpenGL4;

namespace Core.Shaders;

internal class LampShader : ShaderBase
{
    public LampShader()
    {
        Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag", "lamp");
    }

    /// <inheritdoc />
    public override void SetAttributes()
    {
        var positionLocation = Shader.GetAttribLocation(ShaderAttributes.Pos);
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);
    }
}
