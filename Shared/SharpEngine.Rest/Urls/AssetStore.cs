namespace SharpEngine.Rest.Urls;

public static class AssetStore
{
    public const string BaseUrl = "https://assetstore.sharpengine.com/";
    public static string GetById(Guid assetId) => $"assets/{assetId}";
}
