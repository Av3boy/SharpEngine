using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Database.Models;

/// <summary>
///     Represents a database model for a comment made by a user on an asset.
/// </summary>
public class CommentModel
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
    public CommentModel? ParentComment { get; init; }

    /// <summary>
    ///     Gets or initializes the replies to this comment.
    /// </summary>
    public IReadOnlyList<CommentModel> Replies { get; init; } = [];

    /// <summary>
    ///     Converts the comment's database representation to a data transfer object.
    /// </summary>
    /// <param name="model">The model to be converted</param>
    public static explicit operator CommentDto?(CommentModel? model)
        => model is null ? null : new()
        {
            Id = model!.Id,
            AssetId = model.AssetId,
            UserId = model.UserId,
            Content = model.Content,
            CreatedAt = model.CreatedAt,
            ParentComment = model.ParentComment is null ? null : (CommentDto)model.ParentComment!,
            Replies = model.Replies.Count > 0 ? model.Replies.Select(replyModel => (CommentDto)replyModel!).ToList() : []

        };

    /// <summary>
    ///     Converts the comment's data transfer object to its database representation.
    /// </summary>
    /// <param name="dto">The data transfer object to be converted.</param>
    public static explicit operator CommentModel?(CommentDto? dto)
        => dto is null ? null : new()
        {
            Id = dto!.Id,
            AssetId = dto.AssetId,
            UserId = dto.UserId,
            Content = dto.Content,
            CreatedAt = dto.CreatedAt,
            ParentComment = dto.ParentComment is null ? null : (CommentModel?)dto.ParentComment,
            Replies = dto.Replies.Count > 0 ? dto.Replies.Select(replyDto => (CommentModel)replyDto!).ToList() : []
        };
}
