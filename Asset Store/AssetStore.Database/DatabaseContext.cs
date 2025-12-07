using AssetStore.Database.Models;
using Microsoft.EntityFrameworkCore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Database;

public class DatabaseContext : DbContext
{
    public DbSet<AssetModel> Assets { get; set; }
    public DbSet<CommentModel> Comments { get; set; }

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

        modelBuilder.Entity<AssetModel>(entity =>
        {
            entity.HasMany(asset => asset.Comments);

            entity.Property(u => u.Id)
                  .HasConversion(
                    id => id.Value,
                    value => new AssetId(value));
        });

        modelBuilder.Entity<CommentModel>(entity => 
        {
            entity.Property(u => u.Id)
                  .HasConversion(
                    id => id.Value,
                    value => new CommentId(value));

        });
    }
}