using Minecraft.Block;

using OpenTK.Mathematics;

using System;

namespace Minecraft
{
    /// <summary>
    ///     Used to create a block objects.
    /// </summary>
    public static class BlockFactory
    {
        /// <summary>
        ///     Creates a new block object.
        /// </summary>
        /// <param name="type">The type of the block to be created.</param>
        /// <param name="position">Where the block should be created.</param>
        /// <param name="name">The name of the block to be created.</param>
        /// <returns>The newly created block.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static BlockBase CreateBlock(BlockType type, Vector3 position, string name)
            => type switch
            {
                BlockType.Dirt => new Dirt(position, name),
                BlockType.Stone => new Stone(position, name),
                _ => throw new ArgumentException("Invalid block type", nameof(type)),
            };
    }

    /// <summary>
    ///     Represents the type of block.
    /// </summary>
    public enum BlockType
    {
        /// <summary>Represents no block type.</summary>
        None,

        /// <summary>Represents a dirt block.</summary>
        Dirt,

        /// <summary>Represents a stone block.</summary>
        Stone
    }
}
