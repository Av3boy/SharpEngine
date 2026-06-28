namespace SharpEngine.Core.Numerics;

/// <summary>
///     Provides mathematical utility functions.
/// </summary>
public static partial class MathExtensions
{
    /// <summary>
    ///     Returns the largest integer less than or equal to the specified float value.
    /// </summary>
    /// <param name="value">The float value.</param>
    /// <returns>The largest integer less than or equal to the specified float value.</returns>
    public static int FastFloor(float value)
    {
        int integer = (int)value;

        return value < integer ? integer - 1 : integer;
    }

    /// <summary>
    ///   Smooths the input value.
    /// </summary>
    /// <param name="t">The input value.</param>
    /// <returns>The smoothed value.</returns>
    public static float Fade(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    /// <summary>
    ///    Performs linear interpolation between two values.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="t">The interpolation factor.</param>
    /// <returns>The interpolated value.</returns>
    public static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    /// <summary>
    ///     Computes the gradient for a given hash and coordinates.
    /// </summary>
    /// <param name="hash">The hash value.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    /// <returns>The computed gradient.</returns>
    public static float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;

        float u = h < 8 ? x : y;
        float v = h < 4
            ? y
            : h is 12 or 14
                ? x
                : z;

        return ((h & 1) == 0 ? u : -u) +
               ((h & 2) == 0 ? v : -v);
    }
}
