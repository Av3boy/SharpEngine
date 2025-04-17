using SharpEngine.Core._Resources;
using SharpEngine.Core.Extensions;
using System.Numerics;

namespace Minecraft.Block;

internal class Stone : BlockBase
{
    // TODO: #91 Create record for textures files for less parameters
    // TODO: #91 Create const files for these file strings

    /// <summary>
    ///     Initializes a new <see cref="Stone"/> block.
    /// </summary>
    /// <param name="position">The position where the block is created.</param>
    /// <param name="name">The name of the object in the scene.</param>
    public Stone(Vector3 position, string name) : base(position, name, PathExtensions.GetAssemblyPath("Resources\\container2.png"), 
                                                                       PathExtensions.GetAssemblyPath("Resources\\container2_specular.png"),
                                                                       Default.VertexShader,
                                                                       Default.FragmentShader) { }

    /// <inheritdoc />
    public override BlockType BlockType => BlockType.Stone;

    /// <inheritdoc />
    public override bool IsSolid => true;
}
