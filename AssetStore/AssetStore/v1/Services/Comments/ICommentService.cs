using AssetStore.Database.Models;
using SharpEngine.Shared.Dto.AssetStore;

namespace AssetStore.Api.v1.Services.Comments;

/// <summary>
///     Contains definitions for handling comments.
/// </summary>
public interface ICommentService
{
    /// <summary>
    ///     Creates a new comment for a specific asset.
    /// </summary>
    /// <param name="comment">The comment to be added.</param>
    /// <returns>
    ///     A <see cref="Task"/> representing an asynchronous operation.
    ///     The result of the operation contains the created <see cref="CommentDto"/>.
    /// </returns>
    Task<CommentDto> PostComment(CommentDto comment);
}