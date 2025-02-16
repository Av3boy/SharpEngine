using Silk.NET.OpenGL;

namespace Core.Shaders;

internal class UIShader : ShaderBase
{
    public UIShader()
    {
        Shader = new Shader("Shaders/uiShader.vert", "Shaders/uiShader.frag", "ui");
    }

    public override void SetAttributes()
    {
        var vertexLocation = (uint)Shader.GetAttribLocation("aPos");
        Window.GL.EnableVertexAttribArray(vertexLocation);
        Window.GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    }
}
