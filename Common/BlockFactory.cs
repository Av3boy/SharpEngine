using Minecraft.Block;

using OpenTK.Mathematics;

using System;

namespace Minecraft
{
    public static class BlockFactory
    {
        public static BlockBase CreateBlock(BlockType type, Vector3 position, string name)
            => type switch
            {
                BlockType.Dirt => new Dirt(position, name),
                _ => throw new ArgumentException("Invalid block type", nameof(type)),
            };
    }

    public enum BlockType
    {
        Dirt
    }
}
