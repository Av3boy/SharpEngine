using SharpEngine.Shared.Dto.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharpEngine.Shared.Dto.AssetStore;

public class AssetDto
{
    public AssetId Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public IReadOnlyList<string> Tags { get; set; } = [];
    public IReadOnlyList<CommentDto> Comments { get; set; } = [];
    public UserId AuthorId { get; init; }
    public bool Tombstoned { get; set; }
    public Version Version { get; set; }


    // (e.g., model, texture, sound, etc.)
    // public AssetType

    public required string BlobUri { get; set; }
    public required string FileSize { get; set; }

    public required string Checksum { get; set; }
}
