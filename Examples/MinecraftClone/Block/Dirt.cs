using SharpEngine.Core.Extensions;
using System.Numerics;

namespace Minecraft.Block;

/// <summary>
///    Represents a dirt block.
/// </summary>
internal class Dirt : BlockBase
{
    /// <summary>
    ///     Initializes a new <see cref="Dirt"/> block.
    /// </summary>
    /// <param name="position">The position where the block should be initialized.</param>
    /// <param name="name">The name of the block to initialize.</param>
    public Dirt(Vector3 position, string name) : base(position, name, PathExtensions.GetAssemblyPath("Resources\\grass.jpg"), 
                                                                      PathExtensions.GetAssemblyPath("Resources\\container2_specular.png"), 
                                                                      PathExtensions.GetAssemblyPath("Shaders\\shader.vert"), 
                                                                      PathExtensions.GetAssemblyPath("Shaders\\lighting.frag")) { }

    /// <inheritdoc />
    public override BlockType BlockType => BlockType.Dirt;

    /// <inheritdoc />
    public override bool IsSolid => true;
}
