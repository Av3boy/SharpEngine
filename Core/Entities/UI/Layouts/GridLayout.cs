using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using System;

namespace SharpEngine.Core.Entities.UI.Layouts;

/// <summary>
///     Represents a grid layout.
/// </summary>
/// <typeparam name="T">The type of items that can be stored and retrieved within the grid.</typeparam>
public class GridLayout<T> : LayoutBase<T> where T : SceneNode, new()
{
    /// <summary>Gets or sets the amount of rows in the grid.</summary>
    public uint Rows { get; set; } = 2;

    /// <summary>Gets or sets the amount of columns in the grid.</summary>
    public uint Columns { get; set; } = 3;

    /// <summary>
    ///     Retrieves the item at [<paramref name="row"/>, <paramref name="column"/>].
    /// </summary>
    /// <param name="row">The row where the items should be retrieved.</param>
    /// <param name="column">The column where the item should be retrieved.</param>
    /// <returns>The item at [<paramref name="row"/>, <paramref name="column"/>].</returns>
    public T this[uint row, uint column]
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
    public override void AddItem(T item)
    {
        base.AddItem(item);

        var offset = -100; // TODO: Fix this, this is just a temporary solution.

        if (Items.Count % Columns != 0)
            offset = offset + (Items.Count * 30);

        // item.Transform.Position.X = offset;
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
    public override T[][] GetValues()
    {
        var values = new T[Rows][];
        for (uint i = 0; i < Rows; i++)
        {
            values[i] = new T[Columns];
            for (uint j = 0; j < Columns; j++)
            {
                var item = this[i, j];
                values[i][j] = item;
            }
        }

        return values;
    }
}
