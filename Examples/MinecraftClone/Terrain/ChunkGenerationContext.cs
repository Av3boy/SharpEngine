using SharpEngine.Core.Numerics;
using SharpEngine.Core.Numerics.Noise;

namespace Minecraft.Terrain;

public sealed class ChunkGenerationContext
{
    public required int Seed { get; init; }

    public required Vector3 ChunkPosition { get; init; }

    public required int ChunkSize { get; init; }

    public required int MinY { get; init; }

    public required int MaxY { get; init; }

    public required int WaterLevel { get; init; }

    public required INoiseGenerator TerrainNoise { get; init; }

    public required INoiseGenerator CaveNoise { get; init; }

    public required INoiseGenerator OreNoise { get; init; }

    public required INoiseGenerator FoliageNoise { get; init; }
}