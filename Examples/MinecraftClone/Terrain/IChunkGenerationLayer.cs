namespace Minecraft.Terrain;

public interface IChunkGenerationLayer
{
    void Generate(ChunkGenerationContext context, ChunkData chunk);
}