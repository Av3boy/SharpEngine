using System.Collections.Generic;

namespace Minecraft;

public class Inventory
{
    public Dictionary<BlockType, int> Blocks { get; set; } = new();
}
