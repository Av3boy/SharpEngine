using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using System;

namespace SharpEngine.Core.Entities.UI.Layouts;

/// <summary>
///     Represents a layout that dynamically distributes items in a flex layout.
/// </summary>
/// <typeparam name="TItem">The type of items in the collection.</typeparam>
/// <typeparam name="TVector">The amount of dimensions in the content area.</typeparam>
public class FlexLayout<TItem, TVector> : LayoutBase<TItem> where TItem : UIElement, new() where TVector : IVector, new()
{
    /// <summary>Gets or sets the gap between each object.</summary>
    public int Gap { get; set; }

    /// <summary>Gets or sets the dimensions of the content area.</summary>
    public required TVector ContentAreaSize { get; set; }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public override TItem[][] GetValues() => throw new NotImplementedException();
}
