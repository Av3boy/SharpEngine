using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using System.Collections.Generic;

namespace SharpEngine.Core.Entities.UI.Layouts;

/// <summary>
///     Represents the base class for all layout types.
/// </summary>
/// <typeparam name="TItem">The type of the items in the layout.</typeparam>
public abstract class LayoutBase<TItem> : EmptyNode<Transform2D, Vector2> where TItem : UIElement
{
    protected LayoutBase() : this($"{typeof(TItem).Name} Layout") { }
    protected LayoutBase(string name) : base(name) { }

    /// <summary>Gets or sets the distance between items in the grid.</summary>
    public Vector2 Spacing { get; set; } = new(30, 30);

    /// <summary>
    ///     Adds a new item to the layout.
    /// </summary>
    /// <param name="nodes">The items to be added.</param>
    /// <returns>The object itself.</returns>
    public override LayoutBase<TItem> AddChild(params SceneNode[] nodes)
    {
        base.AddChild(nodes);

        foreach (var node in nodes)
            AddChild(node);

        return this;
    }

    /// <summary>
    ///     Gets the items in the container.
    /// </summary>
    /// <returns>The items as a 2D array representation.</returns>
    public abstract TItem[][] GetValues();
}
