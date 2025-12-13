using SharpEngine.Shared.Dto.AssetStore;
using SharpEngine.Shared.Dto.Primitives;

namespace AssetStore.Api.v1.Services.Asset;

/// <summary>
///     Contains definitions for handling assets.
/// </summary>
public interface IAssetService
{
    /// <summary>
    ///     Creates a new asset.
    /// </summary>
    /// <remarks>
    ///     Some of the asset's properties will be set during creation.
    /// </remarks>
    /// <param name="asset">The asset to be created.</param>
    /// <returns>The new created asset.</returns>
    Task<AssetDto> CreateAsset(AssetDto asset);

    /// <summary>
    ///     Deletes an asset by the asset's Id.
    /// </summary>
    /// <param name="assetId">The id of the asset that should be deleted.</param>
    /// <returns><see langword="true"/> when the asset was successfully removed; otherwise. <see langword="false"/>.</returns>
    Task<bool> DeleteAsset(AssetId assetId);

    /// <summary>
    ///     Gets an asset by the asset's Id.
    /// </summary>
    /// <param name="assetId">The id of the asset to be found.</param>
    /// <returns>The asset if found; otherwise <see langword="null"/>.</returns>
    Task<AssetDto?> GetAsset(AssetId assetId);

    /// <summary>
    ///     Gets all the assets.
    /// </summary>
    /// <returns>All the assets found.</returns>
    IEnumerable<AssetDto> GetAssets();

    /// <summary>
    ///     Gets all the assets that match the given keywords.
    /// </summary>
    /// <param name="keywords">The keywords the asset should contain.</param>
    /// <returns>All the assets found with the keyword.</returns>
    IEnumerable<AssetDto> GetAssets(IReadOnlyList<string> keywords);

    /// <summary>
    ///     Gets all the assets created by the given author.
    /// </summary>
    /// <param name="authorId">The id of the author whose assets should be found.</param>
    /// <returns>All the assets found for the author.</returns>
    IEnumerable<AssetDto> GetAssets(UserId authorId);

    /// <summary>
    ///     Finds an asset by its checksum.
    /// </summary>
    /// <param name="checksum">The checksum by which the asset should be found.</param>
    /// <returns>The asset if one exists; otherwise, <see langword="null"/>.</returns>
    AssetDto? GetByChecksum(string checksum);

    /// <summary>
    ///     Creates a revision for the given asset.
    /// </summary>
    /// <remarks>
    ///     NOTE: If the asset does not exist, a new asset will be created.
    /// </remarks>
    /// <param name="asset">The details of the new asset.</param>
    /// <returns>The revised asset.</returns>
    AssetDto ReviseAsset(AssetDto asset);
}