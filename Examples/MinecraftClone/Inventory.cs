using System.Linq;

namespace Minecraft;

public class Inventory
{
    public int SelectedSlotIndex { get; set; } = 1;
    public ToolbarSlot SelectedSlot { get; set; } = new(1);

    // TODO: After first version
    // public List<InventoryItem> Items { get; set; } = new();
    public ToolbarSlot[] Toolbar { get; set; } = new ToolbarSlot[10];

    // TODO: Use later to opimize inventory logic
    private bool _toolbarSlotsAvailable;

    public InventoryItem GetActiveSlotItems(int slotNumber)
        => Toolbar[slotNumber].Items;

    /*public void AddItem(BlockType type)
    {
        var item = Items.FirstOrDefault(x => x.Type == type);
        if (item is not null)
            item.Amount += 1;
        else
            Items.Add(new InventoryItem { Type = type, Amount = 1 });
    }*/

    public void AddToolbarItem(BlockType blockType)
    {
        var slot = Toolbar.FirstOrDefault(i => i.Items.Type == blockType);
        if (slot is not null)
        {
            slot.Items.Amount += 1;

            if (slot.SlotNumber == SelectedSlotIndex)
                SelectedSlot = slot;

            return;
        }

        if (SelectedSlot.Items.Type == BlockType.None)
        {
            SelectedSlot.Items = new InventoryItem { Type = blockType, Amount = 1 };
            Toolbar[SelectedSlotIndex] = SelectedSlot;
            return;
        }

        for (int i = 0; i < Toolbar.Length; i++)
        {
            if (Toolbar[i].Items.Type == BlockType.None)
            {
                Toolbar[i].Items = new InventoryItem { Type = blockType, Amount = 1 };
                return;
            }
        }
    }

    public void RemoveToolbarItem()
    {

    }

    public void Initialize()
    {
        for (int i = 0; i < Toolbar.Length; i++)
            Toolbar[i] = new(i);
    }

    public void SetSelectedSlot(int slotNumber)
    {
        SelectedSlotIndex = slotNumber;
        SelectedSlot = Toolbar[slotNumber];
    }
}

public class ToolbarSlot
{
    public InventoryItem Items { get; set; } = new();
    public int SlotNumber { get; set; }

    public ToolbarSlot(int slotNumber)
    {
        SlotNumber = slotNumber;
    }
}

public class InventoryItem
{
    public BlockType Type { get; set; } = BlockType.None;
    public int Amount { get; set; }
}