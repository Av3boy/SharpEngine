using SharpEngine.Core.Scenes;
using System;
using System.Numerics;

namespace SharpEngine.Core.Entities.UI.Layouts;

/// <summary>
///     Represents a layout that dynamically distributes items in a flex layout.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class FlexLayout<T> : LayoutBase<T> where T : SceneNode, new()
{
    public int Gap { get; set; }

    public int ContentSize { get; set; }

    public Vector2 ContentAreaSize { get; set; }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public override T[][] GetValues() => throw new NotImplementedException();
}
