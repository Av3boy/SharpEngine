using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

public class DefaultShader : Shader
{
    public DefaultShader(GL gl) : base(gl, _Resources.Default.VertexShader, _Resources.Default.FragmentShader, "default") { }

    // public override bool SetAttributes() => true;
}
