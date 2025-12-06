using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Database.Models;

public class Asset
{
    public AssetId Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public IReadOnlyList<string> KeyWords { get; set; } = [];
    public IReadOnlyList<Comment> Comments { get; set; } = [];
    public UserId AuthorId { get; set; }
    public bool Tombstoned { get; set; } = false;
    public Version Version { get; set; }
    // (e.g., model, texture, sound, etc.)
    // public AssetType

    public required string BlobUri { get; set; }
    public required string FileSize { get; set; }

    public required string Checksum { get; set; }

    public static explicit operator Asset(AssetDto dto)
    {
        throw new NotImplementedException();
    }

    public static explicit operator AssetDto(Asset? model)
    {
        throw new NotImplementedException();
    }
}