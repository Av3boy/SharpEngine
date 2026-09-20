using SharpEngine.Core.Defaults;
using SharpEngine.Core.Numerics;
using SharpEngine.IO.Extensions;

namespace Minecraft.Terrain.Block;

internal class Leaf : BlockBase
{
    public Leaf(Silk.NET.OpenGL.GL gl, Vector3 position, string name)
    : base(gl, position, name, DiffuseMap(), SpecularMap(), Defaults.VertexShader, Defaults.FragmentShader) { }

    private static string DiffuseMap() => PathExtensions.GetAssemblyPath("Resources\\container2.png");
    private static string SpecularMap() => PathExtensions.GetAssemblyPath("Resources\\container2_specular.png");

    /// <inheritdoc />
    public override BlockId BlockId => BlockId.Leaves;
}
