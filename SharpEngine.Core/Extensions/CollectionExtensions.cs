using System.Collections.Generic;
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
    /// <param name="items">The collection where the item's existence is being checked.</param>
    /// <returns><see langword="true"/> if the item is in the collection; otherwise, <see langword="false"/>.</returns>
    public static bool IsAnyOf<T>(this T item, params T[] items)
        => items.Contains(item);

    /// <summary>
    ///     Adds the specified items to the collection.
    /// </summary>
    /// <remarks>
    ///     The <see cref="IEnumerable{T}"/> cannot be modified directly, so a new collection is created with the items added.
    /// </remarks>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="collection">The collection to be updated</param>
    /// <param name="items">The items to be added.</param>
    /// <returns>The collection with the items added.</returns>
    public static IEnumerable<T> AddRange<T>(this IEnumerable<T> collection, params T[] items)
    {
        foreach (var item in items)
            collection = collection.Append(item);

        return collection;
    }
}