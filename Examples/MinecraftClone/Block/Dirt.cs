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
    /// <param name="position"></param>
    /// <param name="name"></param>
    public Dirt(Vector3 position, string name)
        : base(position, name, "Resources/grass.jpg", "Resources/container2_specular.png", "Shaders/shader.vert", "Shaders/lighting.frag") { }

    /// <inheritdoc />
    public override BlockType BlockType => BlockType.Dirt;

    /// <inheritdoc />
    public override bool IsSolid => true;
}
