using Silk.NET.Input;
using System;

namespace Core.Enums;

/// <summary>
///     Contains the possible mouse wheel scroll directions.
/// </summary>
public enum MouseWheelScrollDirection
{
    /// <summary>The wheel was scrolled up.</summary>
    Up,

    /// <summary>The wheel was scrolled down.</summary>
    Down
}

/// <summary>
///     Contains extension methods for the <see cref="MouseWheelScrollDirection"/> enum.
/// </summary>
public static class MouseWheelScrollDirectionExtensions
{
    /// <summary>
    ///    Converts the <see cref="ScrollWheel"/> to a <see cref="MouseWheelScrollDirection"/>.
    /// </summary>
    /// <param name="sw">The scroll wheel.</param>
    /// <returns>A <see cref="MouseWheelScrollDirection"/> representation of the direction of the scroll wheel.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static MouseWheelScrollDirection ToString(this ScrollWheel sw)
        => sw.Y switch
        {
            > 0 => MouseWheelScrollDirection.Up,
            < 0 => MouseWheelScrollDirection.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(sw), sw, null)
        };
}