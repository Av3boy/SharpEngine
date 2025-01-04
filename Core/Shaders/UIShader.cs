using OpenTK.Graphics.OpenGL4;

namespace Core.Shaders;
internal class UIShader : ShaderBase
{
    public UIShader()
    {
        Shader = new Shader("Shaders/uiShader.vert", "Shaders/uiShader.frag", "ui");
    }

    public override void SetAttributes()
    {
        var vertexLocation = Shader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    }
}
