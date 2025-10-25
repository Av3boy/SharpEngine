using AssetStore.Database.Models;
using Microsoft.EntityFrameworkCore;

public class BloggingContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public BloggingContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {

    }
}