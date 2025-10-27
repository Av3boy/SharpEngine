using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Database.Models;

public class Comment
{
    public CommentId Id { get; set; }
    public AssetId AssetId { get; set; }
    public UserId UserId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Comment? ParentComment { get; set; }
    public IReadOnlyList<Comment> Replies { get; set; } = [];
}
