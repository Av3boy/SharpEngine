using AssetStore.Database.Models;
using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Services.Asset;

public interface IAssetService
{
    Task<AssetDto> CreateAsset(AssetDto asset);
    Task<bool> DeleteAsset(AssetId assetId);
    Task<AssetDto> GetAsset(AssetId assetId);
    IEnumerable<AssetDto> GetAssets();
    IEnumerable<AssetDto> GetAssets(IReadOnlyList<string> keywords);
    IEnumerable<AssetDto> GetAssets(UserId authorId);
    AssetDto GetByChecksum(string checksum);
    AssetDto ReviseAsset(AssetDto asset);
}