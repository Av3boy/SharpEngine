using Microsoft.EntityFrameworkCore;
using Portal.Database.Models;

public class BloggingContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievements> UserAchievements { get; set; }

    public BloggingContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {

    }
}