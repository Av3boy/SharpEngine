using System.Numerics;

namespace SharpEngine.Core.Entities.Properties;

public interface ITransform<IVector>
{
    public abstract IVector Position { get; set; }
    public abstract IVector Scale { get; set; }
    public abstract Quaternion Rotation { get; set; }
    public abstract Matrix4x4 ModelMatrix { get; }
}