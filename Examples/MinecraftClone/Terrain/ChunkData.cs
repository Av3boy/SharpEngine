using Minecraft.Terrain.Block;

namespace Minecraft.Terrain;

public sealed class ChunkData
{
    private readonly BlockId[,,] _blocks;
    private readonly int[,] _heightMap;

    public ChunkData(int size, int height)
    {
        Size = size;
        Height = height;

        _blocks = new BlockId[size, height, size];
        _heightMap = new int[size, size];
    }

    public int Size { get; }

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
               x < Size &&
               y < Height &&
               z < Size;
    }

    public bool IsAir(int x, int y, int z)
    {
        return GetBlock(x, y, z) == BlockId.Air;
    }
}