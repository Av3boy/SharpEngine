using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Extensions;
using Silk.NET.OpenGL;
using System;

namespace SharpEngine.Core.Shaders;

internal class LightingShader : ShaderBase
{
    public LightingShader()
    {
        Shader = ShaderService.Instance.LoadShader(PathExtensions.GetAssemblyPath("Shaders/shader.vert"), PathExtensions.GetAssemblyPath("Shaders/lighting.frag"), "lighting");

        Vao = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(Vao);

        SetAttributes();
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false;

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
            return false;

        var positionLocationUint = (uint)positionLocation;
        Window.GL.EnableVertexAttribArray(positionLocationUint);
        Window.GL.VertexAttribPointer(positionLocationUint, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);
        
        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Normal, out int normalLocation))
            return false;
        
        var normalLocationUint = (uint)normalLocation;
        Window.GL.EnableVertexAttribArray(normalLocationUint);
        Window.GL.VertexAttribPointer(normalLocationUint, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);
        
        if (!Shader!.TryGetAttribLocation(ShaderAttributes.TexCoords, out int texCoordLocation))
            return false;
        
        var texCoordLocationUint = (uint)texCoordLocation;
        Window.GL.EnableVertexAttribArray(texCoordLocationUint);
        Window.GL.VertexAttribPointer(texCoordLocationUint, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);

        return true;
    }
}
