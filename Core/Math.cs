using System;
using System.Numerics;

namespace Core
{
    public static class Math
    {
        /// <summary>
        ///     Converts the given <paramref name="matrix"/> to a <see cref="System.ReadOnlySpan{T}"/> representation.
        /// </summary>
        /// <param name="matrix">The matrix to be converted.</param>
        /// <returns>A ReadOnlySpan of floats representing the matrix.</returns>
        public static ReadOnlySpan<float> ToSpan(this Matrix4x4 matrix)
            => new ReadOnlySpan<float>([
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            ]);
    }
}
