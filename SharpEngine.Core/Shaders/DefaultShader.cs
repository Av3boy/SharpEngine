using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

public class DefaultShader : Shader
{
    public DefaultShader(GL gl) : base(gl, Defaults.Defaults.VertexShader, Defaults.Defaults.FragmentShader, "default") { }

    // public override bool SetAttributes() => true;
}
