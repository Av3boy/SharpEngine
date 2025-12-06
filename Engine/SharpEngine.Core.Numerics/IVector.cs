namespace SharpEngine.Core.Numerics;

/// <summary>
///     An abstraction of the different vector types.
/// </summary>
public interface IVector
{
    /// <inheritdoc cref="System.Numerics.Vector3.X" />
    float X { get; set; }

    /// <inheritdoc cref="System.Numerics.Vector3.Y" />
    float Y { get; set; }

    public const float Epsilon = 1e-5f;
}