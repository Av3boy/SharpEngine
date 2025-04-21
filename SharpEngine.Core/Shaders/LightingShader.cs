using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class LightingShader : ShaderBase
{
    /// <summary>
    ///     Initializes a new instance of <see cref="LightingShader" />.
    /// </summary>
    public LightingShader()
    {
        Shader = ShaderService.Instance.LoadShader(Default.VertexShader, Default.FragmentShader, "lighting").Initialize();

        Vao = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(Vao);

        SetAttributes();
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false;

        /*if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int positionLocation))
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
        */
        return true;
    }
}
