using System.Collections.Generic;

namespace Minecraft;

public class Inventory
{
    // TODO: UI for inventory

    public Dictionary<BlockType, int> Blocks { get; set; } = new();
}
