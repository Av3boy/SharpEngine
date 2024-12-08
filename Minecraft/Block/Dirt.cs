using OpenTK.Mathematics;

namespace Minecraft.Block;

internal class Dirt : BlockBase
{
    public Dirt(Vector3 position, string name)
        : base(position, name, "Resources/grass.jpg", "Resources/container2_specular.png", "Shaders/shader.vert", "Shaders/lighting.frag") { }

    public override BlockType BlockType => BlockType.Dirt;

    public override bool IsSolid => true;
}
