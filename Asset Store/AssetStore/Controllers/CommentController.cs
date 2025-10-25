using AssetStore.Database.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
