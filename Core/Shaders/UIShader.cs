using SharpEngine.Core.Extensions;
using SharpEngine.Core.Windowing;
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

        if (!Shader!.TryGetAttribLocation(ShaderAttributes.Pos, out int vertexLocation))
            return false;

        uint vertexLocationUInt = (uint)vertexLocation;
        Window.GL.EnableVertexAttribArray(vertexLocationUInt);
        Window.GL.VertexAttribPointer(vertexLocationUInt, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        return true;
    }
}
