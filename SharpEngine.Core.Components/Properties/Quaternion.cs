using System.Numerics;

namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents a quaternion for rotation.
/// </summary>
public class Quaternion
{
    /// <summary>
    ///     Gets or sets the angle of rotation in degrees.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    ///     Gets or sets the axis of rotation.
    /// </summary>
    public Vector3 Axis { get; set; } = new(0, 1, 0);
}
