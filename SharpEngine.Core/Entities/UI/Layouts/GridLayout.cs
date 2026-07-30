using SharpEngine.Core.Scenes;
using SharpEngine.Core.Entities.UI;
using SharpEngine.Core.Numerics;
using System;

namespace SharpEngine.Core.Entities.UI.Layouts;

/// <summary>
///     Represents a grid layout that automatically updates item transforms under the hood.
/// </summary>
/// <remarks>
///     TODO: This component currently starts at the bottom left corner of the grid.
///     A control to change the starting point of the grid should be added in the future.
/// </remarks>
/// <typeparam name="TItem">The type of items that can be stored and retrieved within the grid.</typeparam>
public class GridLayout<TItem> : LayoutBase<TItem> where TItem : UIElement
{
    private readonly bool _useBounds;
    private readonly Vector2 _endPosition;

    public GridLayout() : this("GridLayout") { }
    
    public GridLayout(Vector2 position) : this("GridLayout") 
    { 
        Transform.Position = position;
    }

    public GridLayout(Vector2 startPosition, Vector2 endPosition) : this("GridLayout")
    {
        Transform.Position = startPosition;
        _endPosition = endPosition;
        _useBounds = true;
    }

    public GridLayout(string name) : base(name) { }

    /// <summary>Gets or sets the amount of rows in the grid.</summary>
    /// <remarks>
    ///     If <see cref="AutoRows"/> is enabled, this value will be overridden and calculated based on the number of items and columns.
    /// </remarks>
    public uint Rows { get; set; } = 1;

    /// <summary>Gets or sets the amount of columns in the grid.</summary>
    /// <remarks>
    ///     If <see cref="AutoColumns"/> is enabled, this value will be overridden and calculated based on the number of items and rows.
    /// </remarks>
    public uint Columns { get; set; } = 1;

    /// <summary>Gets or sets a value indicating whether the number of columns should be automatically calculated.</summary>
    public bool AutoColumns { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether the number of rows should be automatically calculated.</summary>
    public bool AutoRows { get; set; } = false;

    public float Padding { get; set; }

    /// <summary>Gets or sets the spacing between items in the grid.</summary>
    /// <remarks>The item size is taken from each UI element's width and height.</remarks>
    public new Vector2 Spacing { get; set; }

    // TODO: This calculation assumes all components are of the same size. This should be changed to support different sizes in the future.
    public float TotalWidth => (Columns * (Items.Count > 0 ? Items[0].Width : 0)) + ((Columns - 1) * Spacing.X);

    /// <summary>
    ///     Retrieves the item at [<paramref name="row"/>, <paramref name="column"/>].
    /// </summary>
    /// <param name="row">The row where the items should be retrieved.</param>
    /// <param name="column">The column where the item should be retrieved.</param>
    /// <returns>The item at [<paramref name="row"/>, <paramref name="column"/>].</returns>
    public TItem this[uint row, uint column]
    {
        get
        {
            uint index = GetIndex(row, column);
            return Items[(int)index];
        }
        set
        {
            uint index = GetIndex(row, column);
            Items[(int)index] = value;
        }
    }

    /// <inheritdoc />
    public override void AddItem(TItem item)
    {
        base.AddItem(item);

        UpdateItemTransforms();
    }

    private void UpdateItemTransforms()
    {
        if (Columns == 0)
            return;

        if (Items.Count == 0)
            return;

        var item = Items[0];
        var itemWidth = item.Width;
        var itemHeight = item.Height;

        var position = Transform.Position;
        var baseX = position.X;
        var baseY = position.Y;

        if (_useBounds)
        {
            var startX = MathF.Min(Transform.Position.X, _endPosition.X);
            var endX = MathF.Max(Transform.Position.X, _endPosition.X);
            var availableWidth = endX - startX;
            var horizontalOffset = MathF.Max(0, (availableWidth - TotalWidth) / 2f);

            baseX = startX + horizontalOffset + (itemWidth / 2f);
            baseY = Transform.Position.Y + Spacing.Y + (itemHeight / 2f);
        }

        for (var index = 0; index < Items.Count; index++)
        {
            var row = index / (int)Columns;
            var column = index % (int)Columns;
            item = Items[index];
            position = new Vector2(
                baseX + (column * (itemWidth + Spacing.X)),
                baseY + (row * (itemHeight + Spacing.Y)));

            item.Transform.Position = position;
        }
    }

    /// <summary>
    ///     Calculates the index in the flat list based on row and column.
    /// </summary>
    /// <param name="row">The row where the items should be retrieved.</param>
    /// <param name="column">The column where the item should be retrieved.</param>
    /// <returns>The index of the item at [<paramref name="row"/>, <paramref name="column"/>].</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private uint GetIndex(uint row, uint column)
    {
        if (row < 0 || column < 0 || column >= Columns)
            throw new ArgumentOutOfRangeException(nameof(row), "Invalid row or column index.");

        var index = row * Columns + column;
        if (index >= Items.Count)
            throw new ArgumentOutOfRangeException(nameof(row), "Index exceeds the number of items.");

        return index;
    }

    /// <inheritdoc />
    public override TItem[][] GetValues()
    {
        var values = new TItem[Rows][];
        for (uint i = 0; i < Rows; i++)
        {
            values[i] = new TItem[Columns];
            for (uint j = 0; j < Columns; j++)
            {
                var item = this[i, j];
                values[i][j] = item;
            }
        }

        return values;
    }
}
