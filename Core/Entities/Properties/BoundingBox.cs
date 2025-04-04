using System.Numerics;

namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents a bounding box of a game object.
/// </summary>
public class BoundingBox
{
    /// <summary>
    ///     Gets or sets the minimum point of the bounding box.
    /// </summary>
    public Vector3 Min { get; set; }

    /// <summary>
    ///     Gets or sets the maximum point of the bounding box.
    /// </summary>
    public Vector3 Max { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="min">The minimum point of the box.</param>
    /// <param name="max">The maximum point of the box.</param>
    public BoundingBox(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    ///     Calculates the bounding box of the game object.
    /// </summary>
    /// <returns>The bounding box of the game object.</returns>
    public static BoundingBox CalculateBoundingBox(ITransform<SharpEngine.Core.Numerics.Vector3> transform)
    {
        var min = transform.Position - (transform.Scale / 2);
        var max = transform.Position + (transform.Scale / 2);
        return new BoundingBox((Vector3)min, (Vector3)max);
    }
}
