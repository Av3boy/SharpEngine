using System.Linq;

namespace SharpEngine.Core.Extensions;

/// <summary>
///     Contains extension methods for collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    ///     Checks whether the item is any of the items in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="item">The item which is being checked if it's in the collection.</param>
    /// <param name="items">The collection where the item's existance is being checked.</param>
    /// <returns><see langword="true"/> if the item is in the collection; otherwise, <see langword="false"/>.</returns>
    public static bool IsAnyOf<T>(this T item, params T[] items)
        => items.Contains(item);
}