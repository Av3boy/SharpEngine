namespace AssetStore.Api.Services.Comments;

public interface ICommentService
{
}

public class CommentService : ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly Database.DatabaseContext _db;
    
    public CommentService(ILogger<CommentService> logger, Database.DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }
}