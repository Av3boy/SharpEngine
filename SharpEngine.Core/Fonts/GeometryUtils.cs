using System;
using System.Collections.Generic;
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Fonts;

/// <summary>
/// Geometry utilities for working with bezier curves and polygons used in glyph tessellation.
/// Provides evaluation, splitting and point-in-contour tests used by the tessellator.
/// </summary>
public static class GeometryUtils
{
    public static Vector2 EvaluateQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        var u = 1 - t;
        return new Vector2(u * u * p0.X + 2 * u * t * p1.X + t * t * p2.X,
                           u * u * p0.Y + 2 * u * t * p1.Y + t * t * p2.Y);
    }

    public static Vector2 EvaluateCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        var uuu = uu * u;
        var ttt = tt * t;

        var x = uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X;
        var y = uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Splits a cubic bezier curve at parameter t into two cubic bezier curves.
    /// Returns (left0,left1,left2,left3), (right0,right1,right2,right3) as two arrays of 4 points.
    /// </summary>
    public static (Vector2[] left, Vector2[] right) SplitCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        var p01 = Lerp(p0, p1, t);
        var p12 = Lerp(p1, p2, t);
        var p23 = Lerp(p2, p3, t);

        var p012 = Lerp(p01, p12, t);
        var p123 = Lerp(p12, p23, t);

        var p0123 = Lerp(p012, p123, t);

        var left = new[] { p0, p01, p012, p0123 };
        var right = new[] { p0123, p123, p23, p3 };
        return (left, right);
    }

    public static (Vector2[] left, Vector2[] right) SplitQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        var p01 = Lerp(p0, p1, t);
        var p12 = Lerp(p1, p2, t);
        var p012 = Lerp(p01, p12, t);

        var left = new[] { p0, p01, p012 };
        var right = new[] { p012, p12, p2 };
        return (left, right);
    }

    private static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }

    /// <summary>
    /// Determines whether the given point is inside the polygon defined by the provided points using the non-zero winding rule.
    /// Points must define a closed polygon (first point not repeated).
    /// </summary>
    public static bool IsPointInPolygon(IList<Vector2> polygon, Vector2 point)
    {
        // Non-zero winding number algorithm
        int windingNumber = 0;
        int n = polygon.Count;
        for (int i = 0; i < n; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % n];

            if (a.Y <= point.Y)
            {
                if (b.Y > point.Y && IsLeft(a, b, point) > 0)
                    windingNumber++;
            }
            else
            {
                if (b.Y <= point.Y && IsLeft(a, b, point) < 0)
                    windingNumber--;
            }
        }

        return windingNumber != 0;
    }

    private static float IsLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
    }

    /// <summary>
    /// Flattens a glyph contour (which may contain bezier segments) into a polyline by adaptively subdividing curves.
    /// maxDepth controls recursion depth, tolerance controls flatness.
    /// </summary>
    public static List<Vector2> FlattenContour(List<GlyphSegment> segments, int maxDepth = 8, float tolerance = 0.25f)
    {
        var result = new List<Vector2>();

        foreach (var seg in segments)
        {
            switch (seg.Type)
            {
                case SegmentType.Line:
                    // Points: [p0,p1]
                    result.Add(seg.Points[0]);
                    // ensure the final point is added by next segment or at end
                    break;
                case SegmentType.Quadratic:
                    // approximate quadratic with adaptive subdivision
                    SubdivideQuadratic(seg.Points[0], seg.Points[1], seg.Points[2], result, 0, maxDepth, tolerance);
                    break;
                case SegmentType.Cubic:
                    SubdivideCubic(seg.Points[0], seg.Points[1], seg.Points[2], seg.Points[3], result, 0, maxDepth, tolerance);
                    break;
            }
        }

        // add last point of last segment explicitly
        if (segments.Count > 0)
        {
            var last = segments[segments.Count - 1];
            var pts = last.Points;
            result.Add(pts[pts.Length - 1]);
        }

        return result;
    }

    private static void SubdivideQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, List<Vector2> outPts, int depth, int maxDepth, float tol)
    {
        // flatness metric: distance from midpoint of control to midpoint of chord
        var mid = EvaluateQuadratic(p0, p1, p2, 0.5f);
        var chordMid = new Vector2((p0.X + p2.X) * 0.5f, (p0.Y + p2.Y) * 0.5f);
        var dx = mid.X - chordMid.X;
        var dy = mid.Y - chordMid.Y;
        if (depth >= maxDepth || (dx * dx + dy * dy) <= tol * tol)
        {
            outPts.Add(p0);
            // final point p2 will be added by caller or later
            return;
        }

        var (left, right) = SplitQuadratic(p0, p1, p2, 0.5f);
        SubdivideQuadratic(left[0], left[1], left[2], outPts, depth + 1, maxDepth, tol);
        SubdivideQuadratic(right[0], right[1], right[2], outPts, depth + 1, maxDepth, tol);
    }

    private static void SubdivideCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, List<Vector2> outPts, int depth, int maxDepth, float tol)
    {
        // use distance from cubic midpoint to chord midpoint as flatness
        var mid = EvaluateCubic(p0, p1, p2, p3, 0.5f);
        var chordMid = new Vector2((p0.X + p3.X) * 0.5f, (p0.Y + p3.Y) * 0.5f);
        var dx = mid.X - chordMid.X;
        var dy = mid.Y - chordMid.Y;
        if (depth >= maxDepth || (dx * dx + dy * dy) <= tol * tol)
        {
            outPts.Add(p0);
            return;
        }

        var (left, right) = SplitCubic(p0, p1, p2, p3, 0.5f);
        SubdivideCubic(left[0], left[1], left[2], left[3], outPts, depth + 1, maxDepth, tol);
        SubdivideCubic(right[0], right[1], right[2], right[3], outPts, depth + 1, maxDepth, tol);
    }

    /// <summary>
    /// Determines whether the given point is inside the glyph (considering all contours and winding rule).
    /// </summary>
    public static bool IsPointInGlyph(Glyph glyph, Vector2 point)
    {
        // Apply non-zero winding rule across all contours
        int winding = 0;
        foreach (var contour in glyph.Contours)
        {
            var poly = FlattenContour(contour);
            if (poly.Count < 3) continue;
            if (IsPointInPolygon(poly, point))
                winding++;
        }

        return winding != 0;
    }
}
