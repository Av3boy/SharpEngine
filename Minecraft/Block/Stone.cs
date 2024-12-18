using OpenTK.Mathematics;

namespace Minecraft.Block;

internal class Stone : BlockBase
{
    // TODO: Create record for textures files for less parameters
    // TODO: Create const files for these file strings

    /// <summary>
    ///     Initializes a new <see cref="Stone"/> block.
    /// </summary>
    /// <param name="position">The position where the block is created.</param>
    /// <param name="name">The name of the object in the scene.</param>
    public Stone(Vector3 position, string name)
        : base(position, name, "Resources/container2.png", "Resources/container2_specular.png", "Shaders/shader.vert", "Shaders/lighting.frag") { }

    /// <inheritdoc />
    public override BlockType BlockType => BlockType.Stone;

    /// <inheritdoc />
    public override bool IsSolid => true;
}
