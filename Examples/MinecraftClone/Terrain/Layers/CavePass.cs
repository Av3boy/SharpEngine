using Minecraft.Terrain.Block;

namespace Minecraft.Terrain.Layers;

public sealed class CavePass : IChunkGenerationLayer
{
    private const float CaveThreshold = 0.68f;

    public void Generate(ChunkGenerationContext context, ChunkData chunk)
    {
        for (int x = 0; x < chunk.Size; x++)
        {
            for (int y = 0; y < chunk.Height; y++)
            {
                for (int z = 0; z < chunk.Size; z++)
                {
                    TryCarveBlock(context, chunk, x, y, z);
                }
            }
        }
    }

    private static void TryCarveBlock(
        ChunkGenerationContext context,
        ChunkData chunk,
        int localX,
        int localY,
        int localZ)
    {
        if (localY == 0)
            return;

        BlockId block = chunk.GetBlock(localX, localY, localZ);

        if (block != BlockId.Stone)
            return;

        float worldX = context.ChunkPosition.X + localX;
        float worldY = LocalYToWorldY(context, localY);
        float worldZ = context.ChunkPosition.Z + localZ;

        float caveNoise = context.CaveNoise.Sample3D(worldX, worldY, worldZ);

        if (caveNoise <= CaveThreshold)
            return;

        chunk.SetBlock(localX, localY, localZ, BlockId.Air);
    }

    private static int LocalYToWorldY(ChunkGenerationContext context, int localY)
    {
        return context.MinY + localY;
    }
}