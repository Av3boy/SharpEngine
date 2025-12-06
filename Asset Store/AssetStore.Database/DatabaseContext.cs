using AssetStore.Database.Models;
using Microsoft.EntityFrameworkCore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DatabaseContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreExampleDB;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasMany(asset => asset.Comments);

            entity.Property(u => u.Id)
                  .HasConversion(
                    id => id.Value,
                    value => new AssetId(value));
        });

        modelBuilder.Entity<Comment>(entity => 
        {
            entity.Property(u => u.Id)
                  .HasConversion(
                    id => id.Value,
                    value => new CommentId(value));

        });
    }
}