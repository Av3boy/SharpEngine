namespace SharpEngine.Shared.Extensions;

/// <summary>
///     Contains extensions for reflection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    ///     Gets the value of a property from the given <paramref name="obj"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object whose property value is being retrieved.</param>
    /// <param name="propertyName">The name of the property whose values is being retrieved.</param>
    /// <returns>The value of the property, <see langword="null"/> if not found.</returns>
    public static object? GetPropertyValue<T>(this T obj, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
            return null;

        return property.GetValue(obj);
    }
}
