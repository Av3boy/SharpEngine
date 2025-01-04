namespace Core.Shaders;

public abstract class ShaderBase
{
    public Shader Shader { get; protected set; }

    public abstract void SetAttributes();
}
