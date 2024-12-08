using OpenTK.Mathematics;

namespace Minecraft.Block;

internal class Stone : BlockBase
{
    public Stone(Vector3 position, string name)
        : base(position, name, "Resources/container2.png", "Resources/container2_specular.png", "Shaders/shader.vert", "Shaders/lighting.frag") { }

    public override BlockType BlockType => BlockType.Stone;

    public override bool IsSolid => true;
}
