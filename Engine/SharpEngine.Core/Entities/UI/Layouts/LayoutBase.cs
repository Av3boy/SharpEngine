using SharpEngine.Core.Scenes;
using System.Collections.Generic;

namespace SharpEngine.Core.Entities.UI.Layouts;

/// <summary>
///     Represents the base class for all layout types.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public abstract class LayoutBase<TItem> : SceneNode where TItem : SceneNode, new()
{
    /// <summary>Represents the items in the layout.</summary>
    public List<TItem> Items { get; set; } = [];

    /// <summary>
    ///     Adds a new item to the layout.
    /// </summary>
    /// <param name="nodes">The items to be added.</param>
    /// <returns>The object itself.</returns>
    public override LayoutBase<TItem> AddChild(params SceneNode[] nodes)
    {
        base.AddChild(nodes);

        foreach (var node in nodes)
            AddItem((TItem)node);

        return this;
    }

    /// <summary>
    ///     Adds and item to the container.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public virtual void AddItem(TItem item)
    {
        Items.Add(item);
    }

    /// <summary>
    ///     Gets the items in the container.
    /// </summary>
    /// <returns>The items as a 2D array representation.</returns>
    public abstract TItem[][] GetValues();
}
