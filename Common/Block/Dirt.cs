using OpenTK.Mathematics;

namespace Minecraft.Block;

internal class Dirt : BlockBase
{
    public Dirt(Vector3 position, string name) : base(position, name) { }

    public override bool IsSolid => true;
}
