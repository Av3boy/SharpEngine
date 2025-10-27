namespace Portal.Database.Models;

public class Achievement
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
}
