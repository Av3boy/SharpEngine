using SharpEngine.Shared.Dto.AssetStore;

namespace SharpEngine.Rest.Clients;

public interface IAssetStoreClient
{
    Task<AssetDto> GetAssetAsync(Guid assetId);
}
