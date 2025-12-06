namespace Portal.Database.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public IReadOnlyList<Achievement> Achievements { get; set; } = [];
}
