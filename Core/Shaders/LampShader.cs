using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Shaders;
internal class LampShader
{
    public Shader Shader { get; private set; }

    public LampShader()
    {
        Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag", "lamp");
    }

    public void SetAttributes()
    {
        var positionLocation = Shader.GetAttribLocation(ShaderAttributes.Pos);
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);
    }
}
