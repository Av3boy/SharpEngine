using SharpEngine.Core._Resources;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Numerics;

namespace Minecraft.Terrain.Block;

internal class Stone : BlockBase
{
    // TODO: #91 Create record for textures files for less parameters
    // TODO: #91 Create const files for these file strings

    /// <summary>
    ///     Initializes a new <see cref="Stone"/> block.
    /// </summary>
    /// <param name="position">The position where the block is created.</param>
    /// <param name="name">The name of the object in the scene.</param>
    public Stone(Vector3 position, string name) 
        : base(position, name, DiffuseMap(), SpecularMap(), Default.VertexShader, Default.FragmentShader) { }

    private static string DiffuseMap() => PathExtensions.GetAssemblyPath("Resources\\container2.png");
    private static string SpecularMap() => PathExtensions.GetAssemblyPath("Resources\\container2_specular.png");

    /// <inheritdoc />
    public override BlockId BlockId => BlockId.Stone;

    /// <inheritdoc />
    public override bool IsSolid => true;
}
