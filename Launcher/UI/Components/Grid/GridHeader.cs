namespace Launcher.UI.Components.Grid;

public class GridHeaderInfo
{
    public string Title { get; set; }
    public bool AllowFiltering { get; set; }

    public GridHeaderInfo(string title, bool allowFiltering)
    {
        Title = title;
        AllowFiltering = allowFiltering;
    }
}
