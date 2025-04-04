using SharpEngine.Core.Numerics;
using System.Numerics;

namespace SharpEngine.Core.Entities.Properties;

public interface ITransform<TVector> where TVector : IVector, new()
{
    public TVector Position { get; set; }
    public TVector Scale { get; set; }
    public Quaternion Rotation { get; set; }
    public Matrix4x4 ModelMatrix { get; }
}