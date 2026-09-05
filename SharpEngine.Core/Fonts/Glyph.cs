using System;
using System.Collections.Generic;
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Fonts;

/// <summary>
/// Represents a glyph outline composed of one or more contours. Each contour is a sequence of segments (lines, quadratic or cubic beziers).
/// This class is intentionally lightweight: full TrueType parsing is implemented in later milestones.
/// </summary>
public class Glyph
{
    public string Name { get; }

    /// <summary>
    /// Collection of contours. Each contour is a list of segments.
    /// </summary>
    public List<List<GlyphSegment>> Contours { get; } = new();

    public Glyph(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// Adds a new contour to the glyph.
    /// </summary>
    public void AddContour(IEnumerable<GlyphSegment> segments)
    {
        Contours.Add(new List<GlyphSegment>(segments));
    }

    /// <summary>
    /// Lightweight triangulation stub. Returns zero vertices currently. Full tessellation implemented in future milestones.
    /// </summary>
    public IEnumerable<Vector2> Triangulate()
    {
        // TODO: Implement triangulation (ear clipping, monotone decomposition, or tessellation of bezier curves)
        yield break;
    }
}

/// <summary>
/// A segment within a glyph contour. Can be a straight line, quadratic bezier, or cubic bezier.
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

public enum SegmentType
{
    Line,
    Quadratic,
    Cubic
}
