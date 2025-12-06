namespace SharpEngine.Shared.Dto.Primitives;

public readonly record struct CommentId(Guid Value)
{
    public static CommentId New() => new(Guid.NewGuid());
    public static implicit operator Guid(CommentId id) => id.Value;
    public static implicit operator CommentId(Guid value) => new(value);
}
