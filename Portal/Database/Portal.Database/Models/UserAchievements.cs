namespace Portal.Database.Models;

public class UserAchievements
{
    public User User { get; set; }
    public Achievement Achievement { get; set; }
    public DateTime UnlockedAt { get; set; }
    public int Progress { get; set; }
}
