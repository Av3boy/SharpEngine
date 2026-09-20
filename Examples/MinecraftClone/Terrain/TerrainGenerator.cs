using Minecraft.Terrain.Block;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Lights;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Numerics.Noise;
using SharpEngine.Core.Scenes;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Terrain;

public sealed class TerrainGenerator
{
    private readonly INoiseGenerator _terrainNoise = new PerlinNoiseGenerator(seed: 1234)
    {
        Scale = 32f,
        Octaves = 5,
        Persistence = 0.45f,
        Lacunarity = 2f
    };

    private readonly INoiseGenerator _caveNoise = new PerlinNoiseGenerator(seed: 5678)
    {
        Scale = 18f,
        Octaves = 3,
        Persistence = 0.5f,
        Lacunarity = 2f
    };

    // TODO: Tweak
    private readonly INoiseGenerator _oreNoise = new PerlinNoiseGenerator(seed: 1234)
    {
        Scale = 32f,
        Octaves = 5,
        Persistence = 0.45f,
        Lacunarity = 2f
    };

    // TODO: Tweak
    private readonly INoiseGenerator _foliageNoise = new PerlinNoiseGenerator(seed: 5678)
    {
        Scale = 18f,
        Octaves = 3,
        Persistence = 0.5f,
        Lacunarity = 2f
    };

    private readonly IReadOnlyList<IChunkGenerationLayer> _layers;

    private readonly Scene _scene;

    private SceneNode _lightsNode;
    private SceneNode _blocksNode;
    private readonly bool _minimalDebugSetup;

    private GL _gl;

    internal TerrainGenerator(Scene scene, SceneNode blocksNode, IEnumerable<IChunkGenerationLayer>? layers = null, bool minimalDebugSetup = false)
    {
        _scene = scene;
        _lightsNode = _scene.Root.AddChild<Transform, Vector3>("Lights");
        _blocksNode = blocksNode;
        _layers = layers?.ToArray() ?? Array.Empty<IChunkGenerationLayer>();
        _minimalDebugSetup = minimalDebugSetup;
    }

    internal void InitializeWorld(GL gl)
    {
        _gl = gl;
        InitializeLights();
        InitializeChunks();
    }

    private void InitializeLights()
    {
        _lightsNode.AddChild(new DirectionalLight());

        _lightsNode.AddChild(
            new PointLight(new Vector3(0.7f, 0.2f, 2.0f), 0),
            new PointLight(new Vector3(2.3f, -3.3f, -4.0f), 1),
            new PointLight(new Vector3(-4.0f, 2.0f, -12.0f), 2),
            new PointLight(new Vector3(0.0f, 0.0f, -3.0f), 3)
        );

        _lightsNode.AddChild(new SpotLight()
        {
            Ambient = new Vector3(0.0f, 0.0f, 0.0f),
            Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
            Specular = new Vector3(1.0f, 1.0f, 1.0f),
        });
    }

    private void InitializeChunks()
    {
        int chunkSize = _minimalDebugSetup ? 1 : 16;
        int numChunks = _minimalDebugSetup ? 1 : 5;

        const int seed = 1234;
        const int minY = -16;
        const int maxY = 16;

        for (int chunkZ = 0; chunkZ < numChunks; chunkZ++)
        {
            for (int chunkX = 0; chunkX < numChunks; chunkX++)
            {
                var chunkPos = new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize);

                var context = new ChunkGenerationContext
                {
                    Seed = seed,
                    ChunkPosition = chunkPos,
                    ChunkSize = new Vector3(chunkSize, maxY - minY + 1, chunkSize),
                    MinY = minY,
                    MaxY = maxY,
                    WaterLevel = 0,

                    TerrainNoise = _terrainNoise,
                    CaveNoise = _caveNoise,
                    OreNoise = _oreNoise,
                    FoliageNoise = _foliageNoise
                };

                ChunkData chunk = GenerateChunk(context);

                CreateBlocksFromChunkData(chunk, chunkPos);
            }
        }
    }

    public ChunkData GenerateChunk(ChunkGenerationContext context)
    {
        var chunk = new ChunkData(context.ChunkSize);

        foreach (IChunkGenerationLayer layer in _layers)
        {
            layer.Generate(context, chunk);
        }

        return chunk;
    }

    private void CreateBlocksFromChunkData(ChunkData chunk, Vector3 chunkPos)
    {
        for (int x = 0; x < chunk.Size.X; x++)
        {
            for (int y = 0; y < chunk.Height; y++)
            {
                for (int z = 0; z < chunk.Size.Z; z++)
                {
                    BlockId block = chunk.GetBlock(x, y, z);

                    if (block == BlockId.Air)
                        continue;

                    var blockPos = chunkPos + new Vector3(x, y, z);

                    var blockObject = BlockFactory.CreateBlock(_gl, block, blockPos, $"Block_{blockPos.X}_{blockPos.Y}_{blockPos.Z}");

                    _blocksNode.AddChild(blockObject);
                }
            }
        }
    }
}