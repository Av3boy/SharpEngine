using System;
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Fonts;

/// <summary>
/// A segment within a glyph contour.
/// Can be a straight line, quadratic bezier, or cubic bezier.
/// Points are stored in order: for Line => [p0,p1], Quadratic => [p0,p1,p2], Cubic => [p0,p1,p2,p3]
/// </summary>
public class GlyphSegment
{
    public SegmentType Type { get; }
    public Vector2[] Points { get; }

    public GlyphSegment(SegmentType type, params Vector2[] points)
    {
        Type = type;
        Points = points ?? throw new ArgumentNullException(nameof(points));
    }
}
