using Launcher.UI;
using SharpEngine.Shared.Dto;
using SharpEngine.Shared.Enums;
using SharpEngine.Shared.Extensions;

/// <summary>
///     Handles ordering collections.
/// </summary>
public static class OrderedEnumerableExtensions
{
    /// <summary>
    /// Filters and orders a collection of projects based on specified criteria.
    /// </summary>
    /// <param name="toFilter">The collection of projects to be filtered and ordered.</param>
    /// <param name="filters">The names of properties and their corresponding filter modes.</param>
    /// <returns>A filtered collection of projects based on the applied filters.</returns>
    public static IEnumerable<Project> FilterBy(this IEnumerable<Project> toFilter, Dictionary<string, FilterMode> filters)
    {
        var stringComparer = new NaturalStringComparer();
        IOrderedEnumerable<Project>? ordered = null;

        foreach (var filter in filters)
        {
            string keySelector(Project p) => p.GetPropertyValue(filter.Key)?.ToString() ?? string.Empty;

            if (ordered == null)
                ordered = filter.Value == FilterMode.Ascending
                    ? toFilter.OrderBy(keySelector, stringComparer)
                    : toFilter.OrderByDescending(keySelector, stringComparer);

            else
                ordered = filter.Value == FilterMode.Ascending
                    ? ordered.ThenBy(keySelector, stringComparer)
                    : ordered.ThenByDescending(keySelector, stringComparer);
        }

        return ordered ?? toFilter;
    }
}
