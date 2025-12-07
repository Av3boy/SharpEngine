using SharpEngine.Shared.Dto.Primitives;

namespace SharpEngine.Shared.Dto.AssetStore;

/// <summary>
///   Data Transfer Object representing an asset in the asset store.
/// </summary>
public class AssetDto
{
    /// <summary>Gets or initializes the id of the asset.</summary>
    public AssetId Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public IReadOnlyList<string> KeyWords { get; set; } = [];
    public IReadOnlyList<CommentDto> Comments { get; set; } = [];

    public UserProfile Author { get; init; }
    public UserId AuthorId { get; init; }
    public bool Tombstoned { get; set; }
    public Version Version { get; set; }

    // (e.g., model, texture, sound, etc.)
    // public AssetType

    // {Blob storage base url}/{container}/{blobName}/{author}/{assetId}/{version}/{assetId}
    public required string BlobUri { get; set; }
    public required int FileSizeBytes { get; set; }

    public required string Checksum { get; set; }
}
