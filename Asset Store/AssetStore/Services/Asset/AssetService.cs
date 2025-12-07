using AssetStore.Database.Models;
using Microsoft.EntityFrameworkCore;
using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Services.Asset;

public class AssetService : IAssetService
{
    private readonly ILogger<AssetService> _logger;
    private readonly Database.DatabaseContext _db;

    public AssetService(ILogger<AssetService> logger, Database.DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AssetDto> CreateAsset(AssetDto asset)
    {
        _logger.LogDebug("Creating asset.");

        var model = (AssetStore.Database.Models.Asset)asset;
        await _db.Assets.AddAsync(model);
        await _db.SaveChangesAsync();

        return asset;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsset(AssetId assetId)
    {
        var asset = await _db.Assets.FirstOrDefaultAsync(asset => asset.Id.Value == assetId.Value);
        if (asset is null)
            return false;

        _db.Assets.Remove(asset);
        await _db.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<AssetDto?> GetAsset(AssetId assetId)
    {
        var asset = await _db.Assets.FirstOrDefaultAsync(asset => asset.Id.Value == assetId.Value);
        return (AssetDto?)asset;
    }

    /// <inheritdoc />
    public IEnumerable<AssetDto> GetAssets()
    {
        var assets = _db.Assets.AsNoTracking().Select(asset => (AssetDto)asset); 
        return assets;
    }

    /// <inheritdoc />
    public IEnumerable<AssetDto> GetAssets(IReadOnlyList<string> keywords)
    {
        var assets = _db.Assets.AsNoTracking()
                               .Where(asset => asset.KeyWords.Any(keyword => keywords.Contains(keyword)))
                               .Select(asset => (AssetDto)asset);

        return assets;
    }

    /// <inheritdoc />
    public IEnumerable<AssetDto> GetAssets(UserId authorId)
    {
        var assets = _db.Assets.AsNoTracking()
                       .Where(asset => asset.AuthorId.Value == authorId.Value)
                       .Select(asset => (AssetDto)asset);

        return assets;
    }

    /// <inheritdoc />
    public AssetDto GetByChecksum(string checksum) => throw new NotImplementedException();

    /// <inheritdoc />
    public AssetDto ReviseAsset(AssetDto asset)
    {
        asset.Version = new Version(asset.Version.Major, asset.Version.Minor + 1);

        throw new NotImplementedException();
    }
}
