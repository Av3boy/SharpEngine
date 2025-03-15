using System;
using System.Linq;

namespace Minecraft;

/// <summary>
///     Represents an inventory system with a toolbar of slots.
/// </summary>
public class Inventory
{
    /// <summary>Gets the currently selected inventory slot.</summary>
    public int SelectedSlotIndex { get; set; } = 1;

    /// <summary>Gets or sets the selected slot.</summary>
    public ToolbarSlot SelectedSlot { get; set; } = new(1);

    // TODO: After first version
    // public List<InventoryItem> Items { get; set; } = new();

    /// <summary>Gets or sets the toolbar slots.</summary>
    public ToolbarSlot[] Toolbar { get; set; } = new ToolbarSlot[10];

    // TODO: Use later to optimize inventory logic
    // private bool _toolbarSlotsAvailable;

    /// <summary>
    ///     Gets the item within a given inventory slot.
    /// </summary>
    /// <param name="slotNumber">The index of the slot.</param>
    /// <returns>The item within the slot with the index of <paramref name="slotNumber"/>.</returns>
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

    /// <summary>
    ///     Adds an item to the toolbar.
    /// </summary>
    /// <param name="blockType">The type of the block to add to the toolbar.</param>
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

    /// <summary>
    ///     Removes an item from the toolbar.
    /// </summary>
    public void RemoveToolbarItem() => throw new NotImplementedException();

    /// <summary>Initializes the toolbar.</summary>
    public void Initialize()
    {
        for (int i = 0; i < Toolbar.Length; i++)
            Toolbar[i] = new(i);
    }

    /// <summary>
    ///     Sets the selected slot.
    /// </summary>
    /// <param name="slotNumber">The index of the slot.</param>
    public void SetSelectedSlot(int slotNumber)
    {
        SelectedSlotIndex = slotNumber;
        SelectedSlot = Toolbar[slotNumber];
    }
}

/// <summary>
///     Represents a slot in te toolbar.
/// </summary>
public class ToolbarSlot
{
    /// <summary>
    ///     Gets or sets the items within a slot.
    /// </summary>
    public InventoryItem Items { get; set; } = new();

    /// <summary>
    ///     Gets or sets the number of the slot.
    /// </summary>
    public int SlotNumber { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="ToolbarSlot" />.
    /// </summary>
    /// <param name="slotNumber">The index of the slot.</param>
    public ToolbarSlot(int slotNumber)
    {
        SlotNumber = slotNumber;
    }
}

/// <summary>
///     Represents an item.
/// </summary>
public class InventoryItem
{
    /// <summary>
    ///     Gets or sets the type of the block in the inventory.
    /// </summary>
    public BlockType Type { get; set; } = BlockType.None;

    /// <summary>Gets or sets the amount of the block.</summary>
    public int Amount { get; set; }
}