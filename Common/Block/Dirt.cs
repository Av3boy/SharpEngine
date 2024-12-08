﻿using OpenTK.Mathematics;

namespace Minecraft.Block;

internal class Dirt : BlockBase
{
    public Dirt(Vector3 position, string name) : base(position, name, "Resources/container2.png", "Resources/container2_specular.png") { }

    public override bool IsSolid => true;
}
