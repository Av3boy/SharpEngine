using AssetStore.Api.v1.Services.Comments;
using AssetStore.Database.Models;
using Microsoft.AspNetCore.Mvc;

using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;
using AssetStoreRoutes = SharpEngine.Rest.Urls.AssetStore;

namespace AssetStore.Api.v1.Controllers;

/// <summary>
///     Represents a REST API controller for handling comments.
/// </summary>
[ApiController]
[Route(AssetStoreRoutes.CommentsRoute)]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentService _commentService;

    /// <summary>
    ///     Initializes a new instance of <see cref="CommentController"/>.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="commentService"></param>
    public CommentController(ILogger<CommentController> logger, ICommentService commentService)
    {
        _logger = logger;
        _commentService = commentService;
    }

    /// <summary>
    ///     Adds a comment to an asset.
    /// </summary>
    /// <param name="comment">The comment to be created.</param>
    /// <returns>The created comment.</returns>
    [HttpPost]
    public async Task<CommentDto> PostComment([FromBody] CommentDto comment)
    {
        _logger.LogDebug("Posting comment for asset {AssetId} by user {UserId}", comment.AssetId, comment.UserId);
        var createdComment = await _commentService.PostComment(comment);

        _logger.LogDebug("Posted comment {CommentId} for asset {AssetId} by user {UserId}", createdComment.Id, comment.AssetId, comment.UserId);
        return createdComment;
    }
}
