using System;

namespace SharpEngine.Core.Numerics;

/// <summary>
///     An abstraction of the different vector types.
/// </summary>
public interface IVector
{
    /// <inheritdoc cref="System.Numerics.Vector3.X" />
    public Single X { get; set; }

    /// <inheritdoc cref="System.Numerics.Vector3.Y" />
    public Single Y { get; set; }
}