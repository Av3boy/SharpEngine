using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class UIShader : ShaderBase
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIShader" />.
    /// </summary>
    public UIShader()
    {
        Shader = new Shader(PathExtensions.GetAssemblyPath("Shaders/uiShader.vert"), PathExtensions.GetAssemblyPath("Shaders/uiShader.frag"), "ui");
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false;

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int vertexLocation))
            return false;

        uint vertexLocationUInt = (uint)vertexLocation;
        Window.GL.EnableVertexAttribArray(vertexLocationUInt);
        Window.GL.VertexAttribPointer(vertexLocationUInt, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

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
