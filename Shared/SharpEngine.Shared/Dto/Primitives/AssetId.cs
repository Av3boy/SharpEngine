namespace SharpEngine.Shared.Dto.Primitives;

public readonly record struct AssetId(Guid Value)
{
    public static AssetId New() => new(Guid.NewGuid());
    public static implicit operator Guid(AssetId id) => id.Value;
    public static implicit operator AssetId(Guid value) => new(value);
}
