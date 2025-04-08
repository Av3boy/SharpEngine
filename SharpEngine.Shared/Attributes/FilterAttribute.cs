namespace SharpEngine.Shared.Attributes;

/// <summary>
///     Represents a attribute that indicates if a property is filterable.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FilterAttribute : Attribute
{
    /// <summary>Gets or sets whether the property is filterable in collections.</summary>
    public bool IsFilterable { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="FilterAttribute"/>.
    /// </summary>
    /// <param name="isFilterable">Determines whether the target property is filterable.</param>
    public FilterAttribute(bool isFilterable = true)
    {
        IsFilterable = isFilterable;
    }
}
