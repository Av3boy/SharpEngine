using Minecraft.Terrain.Block;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Entities.Lights;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Numerics.Noise;
using SharpEngine.Core.Scenes;

namespace Minecraft.Terrain;

internal class TerrainGenerator_Old
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

    private readonly Scene _scene;

    private SceneNode _lightsNode;
    private SceneNode _blocksNode;

    internal TerrainGenerator_Old(Scene scene, SceneNode blocksNode)
    {
        _scene = scene;
        _lightsNode = _scene.Root.AddChild<Transform, Vector3>("Lights");
        _blocksNode = blocksNode;
    }

    internal void InitializeWorld()
    {
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
        // TODO: #88 Generate chunks when player moves

        // TODO: #87 Generate chunks using 3d Perlin noise

        const int chunkSize = 16;
        const int numChunks = 1;
        // const int numChunks = 3;

        for (int i = 0; i < numChunks; i++)
        {
            var chunkPos = new Vector3(i * chunkSize, 0, 0);
            GenerateChunk(chunkSize, chunkPos);
        }
    }

    private void GenerateChunk(int chunkSize, Vector3 chunkPos)
    {
        const int minTerrainHeight = 4;
        const int maxTerrainHeight = 16;

        const int dirtDepth = 3;

        const int lowestY = -16;

        const float caveThreshold = 0.68f;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float worldX = chunkPos.X + x;
                float worldZ = chunkPos.Z + z;

                float terrainNoise = _terrainNoise.Sample2D(worldX, worldZ);

                int columnHeight = minTerrainHeight +
                    (int)(terrainNoise * (maxTerrainHeight - minTerrainHeight));

                int surfaceY = (int)chunkPos.Y + columnHeight - 1;
                int bottomY = (int)chunkPos.Y + lowestY;

                for (int worldY = surfaceY; worldY >= bottomY; worldY--)
                {
                    var blockPos = new Vector3(worldX, worldY, worldZ);

                    int depthFromSurface = surfaceY - worldY;

                    if (depthFromSurface == 0)
                    {
                        var grass = new Grass(blockPos, $"Grass ({worldX}, {worldY}, {worldZ})");
                        _blocksNode.AddChild(grass);

                        continue;
                    }

                    if (depthFromSurface <= dirtDepth)
                    {
                        var dirt = new Dirt(blockPos, $"Dirt ({worldX}, {worldY}, {worldZ})");
                        _blocksNode.AddChild(dirt);

                        continue;
                    }

                    bool isLowestLevel = worldY == bottomY;

                    if (!isLowestLevel)
                    {
                        float caveNoise = _caveNoise.Sample3D(worldX, worldY, worldZ);

                        if (caveNoise > caveThreshold)
                            continue;
                    }

                    var stone = new Stone(blockPos, $"Stone ({worldX}, {worldY}, {worldZ})");
                    _blocksNode.AddChild(stone);
                }
            }
        }
    }
}
