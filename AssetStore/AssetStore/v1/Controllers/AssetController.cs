using AssetStore.Api.v1.Services.Asset;
using AssetStore.Database.Models;

using Microsoft.AspNetCore.Mvc;

using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;
using AssetStoreRoutes = SharpEngine.Rest.Urls.AssetStore;

namespace AssetStore.Api.v1.Controllers;

/// <summary>
///     Represents a REST API controller for handling assets.
/// </summary>
[ApiController]
[Route(AssetStoreRoutes.AssetsRoute)]
public class AssetController : ControllerBase
{
    private readonly ILogger<AssetController> _logger;
    private readonly IAssetService _assetService;
    /// <summary>
    ///     Initializes a new instance of <see cref="AssetController"/>.
    /// </summary>
    /// <param name="logger">A logger used to write down executed actions.</param>
    /// <param name="assetService">The service for handling the asset's business logic.</param>
    public AssetController(ILogger<AssetController> logger, IAssetService assetService)
    {
        _logger = logger;
        _assetService = assetService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AssetModel>> GetAllAssets()
    {
        _logger.LogDebug("Retrieving assets.");

        var assets = _assetService.GetAssets();
        _logger.LogDebug("Found {Count} assets.", assets.Count());

        return StatusCode(StatusCodes.Status200OK, assets);
    }

    [HttpGet("{assetId}")]
    public ActionResult<AssetModel> GetById(Guid assetId)
    {
        _logger.LogDebug("Retrieving asset with id '{assetId}'.", assetId);
        var asset = _assetService.GetAsset(new AssetId(assetId));

        if (asset is null)
            return StatusCode(StatusCodes.Status204NoContent, "Unable to find asset.");

        return StatusCode(StatusCodes.Status200OK, asset);
    }

    [HttpGet(AssetStoreRoutes.GET_ALL_BY_KEYWORD)]
    public ActionResult<IEnumerable<AssetModel>> GetAllAssetsByKeyWord([FromBody] IReadOnlyList<string> keywords)
    {
        var joinedKeyWords = string.Join(", ", keywords);
        _logger.LogDebug("Retrieving assets for the given keywords: {KeyWords}.", joinedKeyWords);

        var assets = _assetService.GetAssets(keywords);
        _logger.LogDebug("Found {Count} assets for the given keywords: {KeyWords}.", assets.Count(), joinedKeyWords);

        if (!assets.Any())
            return StatusCode(StatusCodes.Status204NoContent, $"Unable to find assets with keywords '{joinedKeyWords}'.");

        return StatusCode(StatusCodes.Status200OK, assets);
    }

    [HttpGet(AssetStoreRoutes.GET_ALL_BY_AUTHOR)]
    public ActionResult<IEnumerable<AssetModel>> GetAllAssetsByAuthor(Guid authorId)
    {
        _logger.LogDebug("Retrieving assets for author with id: '{Author}'.", authorId);

        var assets = _assetService.GetAssets(authorId);
        _logger.LogDebug("Found {Count} assets for the given author: '{AuthorId}'.", assets.Count(), authorId);

        if (!assets.Any())
            return StatusCode(StatusCodes.Status204NoContent, $"Unable to find assets by author with id '{authorId}'.");

        return StatusCode(StatusCodes.Status200OK, assets);
    }

    [HttpPost]
    public async Task<ActionResult<AssetDto>> CreateAsset([FromBody] AssetDto asset)
    {
        _logger.LogDebug("Checking if asset already exists.");
        if (asset.Id.Value != Guid.Empty)
        {
            _logger.LogDebug("Asset already exists.");
            return ReviseAsset(asset);
        }

        _logger.LogDebug("Creating asset '{Asset}'. Published by '{Author}'.", asset.Name, asset.AuthorId);
        var result = await _assetService.CreateAsset(asset);
        
        _logger.LogDebug("Successfully created asset '{Asset}'. Published by '{Author}'.", asset.Name, asset.AuthorId);
        return result;
    }

    [HttpPut]
    public AssetDto ReviseAsset([FromBody] AssetDto asset)
    {
        // TODO: versioning logic

        // TODO: Replace existing with revised asset
        var result = _assetService.ReviseAsset(asset);

        _logger.LogDebug("Successfully revised asset '{Asset}'. Published by '{Author}'.", asset.Name, asset.AuthorId);
        return asset;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsset(Guid assetId)
    {
        bool removed = await _assetService.DeleteAsset(assetId);
        if (!removed)
            return StatusCode(StatusCodes.Status404NotFound);

        return StatusCode(StatusCodes.Status200OK, "Asset successfully deleted.");
    }
}
