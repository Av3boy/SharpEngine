using Minecraft.Terrain;
using Minecraft.Terrain.Block;
using System;

namespace Minecraft.Terrain.Layers;

public sealed class BaseTerrainPass : IChunkGenerationLayer
{
    private const int DirtDepth = 3;

    public void Generate(ChunkGenerationContext context, ChunkData chunk)
    {
        for (int x = 0; x < chunk.Size; x++)
        {
            for (int z = 0; z < chunk.Size; z++)
            {
                int surfaceWorldY = chunk.GetHeight(x, z);
                int surfaceLocalY = WorldYToLocalY(context, surfaceWorldY);

                GenerateColumn(chunk, x, z, surfaceLocalY);
            }
        }
    }

    private static void GenerateColumn(ChunkData chunk, int x, int z, int surfaceLocalY)
    {
        for (int localY = 0; localY < chunk.Height; localY++)
        {
            BlockId block = GetBlockForLayer(localY, surfaceLocalY);

            chunk.SetBlock(x, localY, z, block);
        }
    }

    private static BlockId GetBlockForLayer(int localY, int surfaceLocalY)
    {
        if (localY > surfaceLocalY)
            return BlockId.Air;

        if (localY == 0)
            return BlockId.Stone;

        int depthFromSurface = surfaceLocalY - localY;

        return depthFromSurface switch
        {
            0 => BlockId.Grass,

            > 0 and <= DirtDepth => BlockId.Dirt,

            _ => BlockId.Stone
        };
    }

    private static int WorldYToLocalY(ChunkGenerationContext context, int worldY)
    {
        return worldY - context.MinY;
    }
}