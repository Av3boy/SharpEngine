using System;
using System.Linq;

namespace Core.Extensions;
public static class EnumerableExtensions
{
    public static bool IsAnyOf<T>(this T item, params T[] items)
        => items.Contains(item);
}
