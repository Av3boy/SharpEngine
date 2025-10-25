namespace Launcher.UI.Components.Grid;

/// <summary>
///     Contains information about a grid header.
/// </summary>
public class GridHeaderInfo
{
    /// <summary>Gets or sets the title of the column.</summary>
    public string Title { get; set; }

    /// <summary>Gets or sets whether the column allows filtering.</summary>
    public bool AllowFiltering { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GridHeaderInfo" />.
    /// </summary>
    /// <param name="title">Specifies the header title for the grid.</param>
    /// <param name="allowFiltering">Indicates whether filtering is permitted on the grid header.</param>
    public GridHeaderInfo(string title, bool allowFiltering)
    {
        Title = title;
        AllowFiltering = allowFiltering;
    }
}
