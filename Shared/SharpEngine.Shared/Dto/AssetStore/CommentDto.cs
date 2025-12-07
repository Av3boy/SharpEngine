using SharpEngine.Shared.Dto.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Shared.Dto.AssetStore;

public class CommentDto
{
    /// <summary>
    ///     Gets or initializes the unique Id of the comment.
    /// </summary>
    public CommentId Id { get; init; }

    /// <summary>
    ///     Gets or initializes the asset Id to which the comment belongs.
    /// </summary>
    public AssetId AssetId { get; init; }

    /// <summary>
    ///     Gets or initializes the user Id of the comment's author.
    /// </summary>
    public UserId UserId { get; init; }

    /// <summary>
    ///     Gets or initializes the content of the comment.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    ///     Gets or initializes the creation date of the comment.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or initializes the parent comment if this comment is a reply.
    /// </summary>
    public CommentDto? ParentComment { get; init; }

    /// <summary>
    ///     Gets or initializes the replies to this comment.
    /// </summary>
    public IReadOnlyList<CommentDto> Replies { get; init; } = [];
}
