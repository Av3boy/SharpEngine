using SharpEngine.Core.Numerics;
using System.Numerics;

namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents the transform of an object in <typeparamref name="TVector"/> dimensions.
/// </summary>
/// <typeparam name="TVector"></typeparam>
public interface ITransform<TVector> where TVector : IVector, new()
{
    /// <summary>Gets or set the the position of the object.</summary>
    public TVector Position { get; set; }

    /// <summary>Gets or set the the scale of the object.</summary>
    public TVector Scale { get; set; }

    /// <summary>Gets or set the the rotation of the object.</summary>
    public Quaternion Rotation { get; set; }

    /// <summary>Gets or set the the model matrix of the object.</summary>
    public Matrix4x4 ModelMatrix { get; }
}