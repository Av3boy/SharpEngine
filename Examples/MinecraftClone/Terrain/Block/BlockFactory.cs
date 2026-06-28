using SharpEngine.Core.Numerics;
using System;

namespace Minecraft.Terrain.Block
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
        public static BlockBase CreateBlock(BlockId type, Vector3 position, string name)
            => type switch
            {
                BlockId.Dirt => new Dirt(position, name),
                BlockId.Stone => new Stone(position, name),
                BlockId.Grass => new Grass(position, name),
                _ => throw new ArgumentException("Invalid block type", nameof(type)),
            };
    }
}
