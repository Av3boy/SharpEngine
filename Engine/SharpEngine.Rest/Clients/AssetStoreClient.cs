using SharpEngine.Rest.Urls;
using SharpEngine.Shared.Dto.AssetStore;

namespace SharpEngine.Rest.Clients;

public class AssetStoreClient : RestClient, IAssetStoreClient
{
    public AssetStoreClient(HttpClient httpClient) : base(httpClient) { }

    public async Task<AssetDto> GetAssetAsync(Guid assetId)
        => await GetAsync<AssetDto>(AssetStore.GetById(assetId));
}
