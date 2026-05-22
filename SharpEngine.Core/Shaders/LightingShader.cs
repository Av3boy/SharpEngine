using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class LightingShader : ShaderBase
{
    private readonly GL _gl;

    /// <summary>
    ///     Initializes a new instance of <see cref="LightingShader" />.
    /// </summary>
    public LightingShader(GL gl)
    {
        _gl = gl;

        Shader = ShaderService.Instance.LoadShader(_gl, Default.VertexShader, Default.FragmentShader, "lighting");

        Vao = _gl.GenVertexArray();
        _gl.BindVertexArray(Vao);

        SetAttributes(_gl);
    }

    /// <inheritdoc />
    public override bool SetAttributes(GL gl)
    {
        if (!base.SetAttributes(gl))
            return false;

        /*if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
            return false;

        var positionLocationUint = (uint)positionLocation;
        _gl.EnableVertexAttribArray(positionLocationUint);
        _gl.VertexAttribPointer(positionLocationUint, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);
        
        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Normal, out int normalLocation))
            return false;
        
        var normalLocationUint = (uint)normalLocation;
        _gl.EnableVertexAttribArray(normalLocationUint);
        _gl.VertexAttribPointer(normalLocationUint, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);
        
        if (!Shader!.TryGetAttribLocation(ShaderAttributes.TexCoords, out int texCoordLocation))
            return false;
        
        var texCoordLocationUint = (uint)texCoordLocation;
        _gl.EnableVertexAttribArray(texCoordLocationUint);
        _gl.VertexAttribPointer(texCoordLocationUint, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);
        */
        return true;
    }
}
