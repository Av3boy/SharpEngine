using Minecraft.Terrain.Block;
using SharpEngine.Core.Numerics;

namespace Minecraft.Terrain;

public sealed class ChunkData
{
    private readonly BlockId[,,] _blocks;
    private readonly int[,] _heightMap;

    public ChunkData(Vector3 size)
    {
        Size = size;
        Height = (int)size.Y;

        _blocks = new BlockId[(int)size.X, Height, (int)size.Z];
        _heightMap = new int[(int)size.X, (int)size.Z];
    }

    public Vector3 Size { get; }

    public int Height { get; }

    public BlockId GetBlock(int x, int y, int z)
    {
        return _blocks[x, y, z];
    }

    public void SetBlock(int x, int y, int z, BlockId block)
    {
        _blocks[x, y, z] = block;
    }

    public int GetHeight(int x, int z)
    {
        return _heightMap[x, z];
    }

    public void SetHeight(int x, int z, int height)
    {
        _heightMap[x, z] = height;
    }

    public bool IsInside(int x, int y, int z)
    {
        return x >= 0 &&
               y >= 0 &&
               z >= 0 &&
               x < Size.X &&
               y < Height &&
               z < Size.Z;
    }

    public bool IsAir(int x, int y, int z)
    {
        return GetBlock(x, y, z) == BlockId.Air;
    }
}