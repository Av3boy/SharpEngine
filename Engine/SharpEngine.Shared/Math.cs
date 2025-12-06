using System;
using System.Numerics;

namespace SharpEngine.Core;

/// <summary>
///     Provides math-related functions.
/// </summary>
public static class Math
{
    /// <summary>
    ///     Converts the given <paramref name="matrix"/> to a <see cref="ReadOnlySpan{T}"/> representation.
    /// </summary>
    /// <param name="matrix">The matrix to be converted.</param>
    /// <returns>A ReadOnlySpan of floats representing the matrix.</returns>
    public static ReadOnlySpan<float> ToSpan(this Matrix4x4 matrix)
        => new([
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        ]);

    /// <summary>Gets the value of π / 2.</summary>
    public static float PiOver2 => MathF.PI / 2;

    /// <summary>
    ///     Converts a value in <paramref name="degrees"/> to radians.
    /// </summary>
    /// <param name="degrees">The degrees to be converted.</param>
    /// <returns>The given <paramref name="degrees"/> in radians.</returns>
    public static double DegreesToRadians(double degrees)
        => System.Math.PI / 180 * degrees;

    /// <inheritdoc cref="DegreesToRadians(double)"/>
    public static float DegreesToRadians(float degrees)
        => MathF.PI / 180 * degrees;

    /// <summary>
    ///    Converts a value in <paramref name="radians"/> to degrees.
    /// </summary>
    /// <param name="radians">The radians to convert.</param>
    /// <returns>The given <paramref name="radians"/> as degrees.</returns>
    public static float RadiansToDegrees(float radians)
        => radians * (180 / MathF.PI);
}
