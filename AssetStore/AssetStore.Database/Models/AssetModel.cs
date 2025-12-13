using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

using System.Collections.Immutable;

namespace AssetStore.Database.Models;

/// <summary>
///     Represents a database model for an asset in the asset store.
/// </summary>
public class AssetModel
{
    /// <summary>
    ///     Gets or initializes the unique Id of the asset.
    /// </summary>
    public AssetId Id { get; init; }

    /// <summary>
    ///     Gets or initializes the name of the asset.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets or initializes the description of the asset.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or initializes the price of the asset.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    ///     Gets or initializes the creation date of the asset.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or initializes the last updated date of the asset.
    /// </summary>
    public DateTime LastUpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or initializes the keywords associated with the asset.
    /// </summary>
    public IReadOnlyList<string> KeyWords { get; init; } = [];

    /// <summary>
    ///     Gets or initializes the comments made on the asset.
    /// </summary>
    public ImmutableList<CommentModel> Comments { get; init; } = [];

    /// <summary>
    ///     Gets or initializes the reactions to the asset.
    /// </summary>
    public Dictionary<int, string> Reactions { get; init; } = new();

    /// <summary>
    ///     Gets or initializes the Id of the asset's author.
    /// </summary>
    public UserId AuthorId { get; init; }

    /// <summary>
    ///     Gets or initializes a value indicating whether the asset is tombstoned (soft-deleted).
    /// </summary>
    /// <remarks>
    ///     Tombstoned assets are not permanently deleted but are marked as inactive or removed from active listings.
    ///     Authors are able to see tombstoned assets and restore them if needed.
    /// </remarks>
    public bool Tombstoned { get; init; } = false;

    /// <summary>
    ///     Gets or initializes the version of the asset.
    /// </summary>
    public required Version Version { get; init; }

    // (e.g., model, texture, sound, etc.)
    // public AssetType AssetType { get; init; }

    /// <summary>
    ///     Gets or initializes the URI to the blob storage where the asset file is stored.
    /// </summary>
    public required string BlobUri { get; init; }

    /// <summary>
    ///     Gets or initializes the size of the asset file.
    /// </summary>
    public required int FileSizeBytes { get; init; }

    /// <summary>
    ///     Gets or initializes the checksum of the asset file.
    /// </summary>
    public required string Checksum { get; init; }

    /// <summary>
    ///     Converts the asset's data transfer object to its database representation.
    /// </summary>
    /// <param name="dto">The data transfer object to be converted.</param>
    public static explicit operator AssetModel?(AssetDto dto)
        => dto is null ? null : new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CreatedAt = dto.CreatedAt,
            LastUpdatedAt = dto.LastUpdatedAt,
            KeyWords = dto.KeyWords,
            Comments = dto.Comments.Count > 0 ? dto.Comments.Select(commentDto => (CommentModel)commentDto!).ToImmutableList() : [],
            AuthorId = dto.AuthorId,
            Tombstoned = dto.Tombstoned,
            Version = dto.Version,
            BlobUri = dto.BlobUri,
            FileSizeBytes = dto.FileSizeBytes,
            Checksum = dto.Checksum
        };

    /// <summary>
    ///     Converts the asset's database representation to a data transfer object.
    /// </summary>
    /// <param name="model">The model to be converted</param>
    public static explicit operator AssetDto?(AssetModel? model)
        => model is null ? null : new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            CreatedAt = model.CreatedAt,
            LastUpdatedAt = model.LastUpdatedAt,
            KeyWords = model.KeyWords,
            Comments = model.Comments.Count > 0 ? model.Comments.Select(commentDto => (CommentDto)commentDto!).ToImmutableList() : [],
            AuthorId = model.AuthorId,
            Tombstoned = model.Tombstoned,
            Version = model.Version,
            BlobUri = model.BlobUri,
            FileSizeBytes = model.FileSizeBytes,
            Checksum = model.Checksum
        };
}