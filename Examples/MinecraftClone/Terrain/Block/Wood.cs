using SharpEngine.Core._Resources;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Numerics;

namespace Minecraft.Terrain.Block;

internal class Wood : BlockBase
{
    public Wood(Vector3 position, string name)
    : base(position, name, DiffuseMap(), SpecularMap(), Default.VertexShader, Default.FragmentShader) { }

    private static string DiffuseMap() => PathExtensions.GetAssemblyPath("Resources\\container2.png");
    private static string SpecularMap() => PathExtensions.GetAssemblyPath("Resources\\container2_specular.png");

    /// <inheritdoc />
    public override BlockId BlockId => BlockId.Wood;
}
