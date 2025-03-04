using System.Linq;

namespace SharpEngine.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsAnyOf<T>(this T item, params T[] items)
            => items.Contains(item);
    }
}