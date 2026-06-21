using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Properties.Meshes;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class LampShader : Shader
{
    /// <summary>
    ///     Initializes a new instance of <see cref="LampShader" /> using the provided OpenGL context.
    /// </summary>
    /// <param name="gl">The OpenGL context to use for shader and VAO creation.</param>
    public LampShader(GL gl) : base(gl, Default.VertexShader, Default.LightShader, nameof(LampShader))
    {
    }

    /// <inheritdoc />
    public override bool SetAttributes(GL gl)
    {
        if (!base.SetAttributes(gl))
            return false;

        if (!TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
            return false;

        var positionLocationUint = (uint)positionLocation;
        GL.EnableVertexAttribArray(positionLocationUint);
        GL.VertexAttribPointer(positionLocationUint, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        return true;
    }
}
