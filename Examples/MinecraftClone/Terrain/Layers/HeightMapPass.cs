namespace Minecraft.Terrain.Layers;

internal class HeightMapPass : IChunkGenerationLayer
{
    public void Generate(ChunkGenerationContext context, ChunkData chunk)
    {
        for (int x = 0; x < chunk.Size; x++)
        {
            for (int z = 0; z < chunk.Size; z++)
            {
                float worldX = context.ChunkPosition.X + x;
                float worldZ = context.ChunkPosition.Z + z;

                float noise = context.TerrainNoise.Sample2D(worldX, worldZ);

                int minSurfaceY = 0;
                int maxSurfaceY = 16;

                int surfaceY = minSurfaceY +
                    (int)(noise * (maxSurfaceY - minSurfaceY));

                chunk.SetHeight(x, z, surfaceY);
            }
        }
    }
}
