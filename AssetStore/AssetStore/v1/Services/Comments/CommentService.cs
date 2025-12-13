using AssetStore.Database.Models;
using Microsoft.EntityFrameworkCore;
using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Api.v1.Services.Comments;

public class CommentService : ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly Database.DatabaseContext _db;
    
    public CommentService(ILogger<CommentService> logger, Database.DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <inheritdoc />
    public async Task<CommentDto> PostComment(CommentDto comment)
    {
        _logger.LogDebug("Posting comment by user '{UserId}' on asset '{AssetId}'.", comment.UserId, comment.AssetId);
        var asset = await _db.Assets.FirstOrDefaultAsync(a => a.Id == comment.AssetId)
            ?? throw new InvalidOperationException($"Asset with ID '{comment.AssetId}' not found.");

        var model = (CommentModel)comment!;
        asset.Comments.Add(model);

        await _db.SaveChangesAsync();
        return (CommentDto)model!;
    }
}