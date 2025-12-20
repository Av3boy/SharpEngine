using SharpEngine.Shared.Dto.Primitives;
namespace SharpEngine.Rest.Urls;

public static class AssetStore
{
    public const string BaseUrl = "https://assetstore.sharpengine.com/";

    public const string Apiv1Route = "api/v1";
    public const string AssetsRoute = Apiv1Route + "/assets";
    public const string CommentsRoute = Apiv1Route + "/comments";
    public const string ChecoutRoute = Apiv1Route + "/checkout";
    
    public static string GetById(Guid assetId) => $"assets/{assetId}";

    public const string GET_ALL_BY_KEYWORD = "/keyword";
    public static string GetAllByAuthor(UserId authorId) => $"/author/{authorId.Value}";
    public const string GET_ALL_BY_AUTHOR = "/author/{authorId}";

    public const string GET_ALL_ASSET_COMMENTS = "/{assetId}/comment";
}