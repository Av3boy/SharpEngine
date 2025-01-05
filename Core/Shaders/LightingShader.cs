using Core.Entities.Properties;
using OpenTK.Graphics.OpenGL4;

namespace Core.Shaders;
internal class LightingShader : ShaderBase
{
    public LightingShader()
    {
        Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
    }

    /// <inheritdoc />
    public override void SetAttributes()
    {
        var positionLocation = Shader.GetAttribLocation(ShaderAttributes.Pos);
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        var normalLocation = Shader.GetAttribLocation(ShaderAttributes.Normal);
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

        var texCoordLocation = Shader.GetAttribLocation(ShaderAttributes.TexCoords);
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);
    }
}
