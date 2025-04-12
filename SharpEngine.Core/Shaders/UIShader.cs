using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;
using SharpEngine.Core._Resources;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class UIShader : ShaderBase
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIShader" />.
    /// </summary>
    public UIShader()
    {
        Shader = new Shader(Window.GL, Default.UIVertexShader, Default.UIFragmentShader, nameof(UIShader)).Initialize();
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
