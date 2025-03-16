using SharpEngine.Core.Extensions;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

internal class UIShader : ShaderBase
{
    public UIShader()
    {
        Shader = new Shader(PathExtensions.GetAssemblyPath("Shaders/uiShader.vert"), PathExtensions.GetAssemblyPath("Shaders/uiShader.frag"), "ui");
    }

    /// <inheritdoc />
    public override bool SetAttributes()
    {
        if (!base.SetAttributes())
            return false; 

        var vertexLocation = Shader!.GetAttribLocation(ShaderAttributes.Pos);
        if (vertexLocation == ShaderAttributes.AttributeLocationNotFound)
            return false;

        uint vertexLocationUInt = (uint)vertexLocation;
        Window.GL.EnableVertexAttribArray(vertexLocationUInt);
        Window.GL.VertexAttribPointer(vertexLocationUInt, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        return true;
    }
}
