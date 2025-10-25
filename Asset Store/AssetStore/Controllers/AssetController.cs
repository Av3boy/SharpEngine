using AssetStore.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly ILogger<AssetController> _logger;

        public AssetController(ILogger<AssetController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Asset> GetAllAssets()
        {
            return new List<Asset>();
        }

        [HttpGet]
        public IEnumerable<Asset> GetAllAssetsByKeyWord()
        {
            return new List<Asset>();
        }

        [HttpGet]
        public IEnumerable<Asset> GetAllAssetsByAuthor()
        {
            return new List<Asset>();
        }

        [HttpPost]
        public Asset CreateAsset(Asset asset)
        {
            return asset;
        }

        [HttpPut]
        public Asset ReviseAsset(Asset asset)
        {
            // TODO: versioning logic

            // TODO: Replace existing with revised asset
            return asset;
        }

        [HttpDelete]
        public bool DeleteAsset(Guid assetId)
        {

            return true;
        }
    }
}
