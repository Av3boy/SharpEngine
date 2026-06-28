namespace SharpEngine.Core.Numerics.Noise;

/// <summary>
///     Defines a noise generator with controllable fractal parameters.
/// </summary>
public interface INoiseGenerator
{
    /// <summary>Gets or sets the scale factor applied to the input coordinates.</summary>
    /// <remarks>
    ///     Higher values make the noise appear more zoomed out.
    /// </remarks>
    float Scale { get; set; }

    /// <summary>Gets or sets the number of octaves (noise layers) to combine.</summary>
    int Octaves { get; set; }

    /// <summary>
    ///     Gets or sets the persistence,
    ///     which controls how much each octave contributes relative to the previous one (amplitude decay per octave).
    /// </summary>
    float Persistence { get; set; }

    /// <summary>
    ///     Gets or sets the lacunarity,
    ///     which controls how quickly the frequency increases per octave.
    /// </summary>
    float Lacunarity { get; set; }

    /// <summary>
    ///     Samples the noise at the given 2D coordinates.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    /// <returns>A normalized value in the range [0, 1].</returns>
    float Sample2D(float x, float z);

    /// <summary>
    ///     Samples the noise at the given 3D coordinates.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    /// <returns>A normalized value in the range [0, 1].</returns>
    float Sample3D(float x, float y, float z);
}
