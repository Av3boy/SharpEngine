using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Properties.Meshes;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class LampShader : ShaderBase
{
    private readonly GL _gl;

    /// <summary>
    ///     Initializes a new instance of <see cref="LampShader" /> using the provided OpenGL context.
    /// </summary>
    /// <param name="gl">The OpenGL context to use for shader and VAO creation.</param>
    public LampShader(GL gl)
    {
        _gl = gl;

        Shader = ShaderService.Instance.LoadShader(_gl, Default.VertexShader, Default.LightShader, "lamp");

        Vao = _gl.GenVertexArray();
        _gl.BindVertexArray(Vao);

        SetAttributes(_gl);
    }

    /// <inheritdoc />
    public override bool SetAttributes(GL gl)
    {
        if (!base.SetAttributes(gl))
            return false;

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
            return false;

        var positionLocationUint = (uint)positionLocation;
        _gl.EnableVertexAttribArray(positionLocationUint);
        _gl.VertexAttribPointer(positionLocationUint, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        return true;
    }
}
