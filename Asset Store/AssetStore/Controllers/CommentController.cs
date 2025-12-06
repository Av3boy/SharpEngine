using AssetStore.Database.Models;
using Microsoft.AspNetCore.Mvc;
using SharpEngine.Shared.Dto.AssetStore;

namespace AssetStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Comment> GetAssetComments(Guid assetId)
        {
            return new List<Comment>();
        }

        [HttpPost]
        public CommentDto PostComment(Guid assetId, CommentDto comment)
        {
            return new CommentDto();
        }
    }
}
