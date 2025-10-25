namespace AssetStore.Database.Models;

public class Asset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime LastUpdatedAt { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = [];
    public IReadOnlyList<Comment> Comments { get; set; } = [];
    public Guid AuthorId { get; set; }
    public bool Tombstoned { get; set; }

    // (e.g., model, texture, sound, etc.)
    // public AssetType

    public required string BlobUri { get; set; }
    public required string FileSize { get; set; }

    public required string Checksum { get; set; }
}