using System;
using System.Collections.Generic;
using System.Linq;
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Fonts;

/// <summary>
///     Represents a glyph outline composed of one or more contours.
/// </summary>
/// <remarks>
///     Each contour is a sequence of segments (lines, quadratic or cubic beziers).
///     TODO: This class is intentionally lightweight: full TrueType parsing is implemented in later milestones.
/// </remarks>
public class Glyph
{
    /// <summary>
    ///     Gets the name of the glyph, typically the character or string it represents.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Collection of contours. Each contour is a list of segments.
    /// </summary>
    public List<List<GlyphSegment>> Contours { get; } = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Glyph"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the glyph.</param>
    /// <exception cref="ArgumentNullException">Thrown when the name is <see langword="null"/>.</exception>
    public Glyph(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    ///     Adds a new contour to the glyph.
    /// </summary>
    /// <param name="segments">The segments that make up the contour.</param>
    public void AddContour(IEnumerable<GlyphSegment> segments) => Contours.Add([.. segments]);

    /// <summary>
    ///     Triangulates the glyph by flattening bezier contours and applying an ear-clipping triangulator per contour.
    /// </summary>
    /// <remarks>
    ///     TODO: This is a simple implementation and does not correctly handle nested holes — outer contours should be provided in CCW order and holes in CW order for correct results.
    /// </remarks>
    public IEnumerable<Vector2> Triangulate(int flattenMaxDepth = 8, float flattenTolerance = 0.25f)
    {
        // Use LibTessDotNet for robust triangulation with holes when available.
        try
        {
            var tess = new LibTessDotNet.Tess();
            foreach (var contour in Contours)
            {
                var poly = GeometryUtils.FlattenContour(contour, flattenMaxDepth, flattenTolerance);
                if (poly.Count < 3)
                    continue;

                var verts = new LibTessDotNet.ContourVertex[poly.Count];
                for (int i = 0; i < poly.Count; i++)
                {
                    verts[i].Position = new LibTessDotNet.Vec3 { X = poly[i].X, Y = poly[i].Y, Z = 0 };
                }

                tess.AddContour(verts, LibTessDotNet.ContourOrientation.Original);
            }

            tess.Tessellate(LibTessDotNet.WindingRule.NonZero, LibTessDotNet.ElementType.Polygons, 3);

            var result = new List<Vector2>();
            var vertices = tess.Vertices;
            var elements = tess.Elements;
            for (int i = 0; i < elements.Length; i += 3)
            {
                if (elements[i] == -1 || elements[i + 1] == -1 || elements[i + 2] == -1)
                    break;

                var a = vertices[elements[i]].Position;
                var b = vertices[elements[i + 1]].Position;
                var c = vertices[elements[i + 2]].Position;
                
                result.Add(new Vector2(a.X, a.Y));
                result.Add(new Vector2(b.X, b.Y));
                result.Add(new Vector2(c.X, c.Y));
            }

            return result;
        }
        catch
        {
            // fallback to simple ear-clipping implementation below
            // TODO: Handle logging.
        }

        var triangles = new List<Vector2>();

        foreach (var contour in Contours)
        {
            var poly = GeometryUtils.FlattenContour(contour, flattenMaxDepth, flattenTolerance);
            if (poly.Count < 3)
                continue;

            var inds = TriangulatePolygon(poly);
            for (int i = 0; i < inds.Count; i += 3)
            {
                triangles.Add(poly[inds[i]]);
                triangles.Add(poly[inds[i + 1]]);
                triangles.Add(poly[inds[i + 2]]);
            }
        }

        return triangles;
    }

    // Simple ear-clipping triangulation. Returns a list of triangle vertex indices into the input polygon.
    private static List<int> TriangulatePolygon(List<Vector2> poly)
    {
        int n = poly.Count;
        var result = new List<int>(System.Math.Max(0, (n - 2) * 3));

        if (n < 3)
            return result;

        if (SignedArea(poly) < 0)
            poly.Reverse();

        Span<int> prev = stackalloc int[n];
        Span<int> next = stackalloc int[n];

        for (int i = 0; i < n; i++)
        {
            prev[i] = i == 0 ? n - 1 : i - 1;
            next[i] = i == n - 1 ? 0 : i + 1;
        }

        int remaining = n;
        int current = 0;
        int guard = 0;

        while (remaining > 3 && guard < 10000)
        {
            bool earFound = false;
            int start = current;

            do
            {
                int p = prev[current];
                int c = current;
                int nx = next[current];

                Vector2 a = poly[p];
                Vector2 b = poly[c];
                Vector2 triangleC = poly[nx];

                if (IsConvex(a, b, triangleC))
                {
                    bool anyInside = false;

                    int j = next[nx];

                    while (j != p)
                    {
                        if (PointInTriangle(poly[j], a, b, triangleC))
                        {
                            anyInside = true;
                            break;
                        }

                        j = next[j];
                    }

                    if (!anyInside)
                    {
                        result.Add(p);
                        result.Add(c);
                        result.Add(nx);

                        // Remove c from the circular linked list.
                        next[p] = nx;
                        prev[nx] = p;

                        current = nx;
                        remaining--;

                        earFound = true;
                        break;
                    }
                }

                current = next[current];

            } while (current != start);

            if (!earFound)
                break;

            guard++;
        }

        if (remaining == 3)
        {
            int a = current;
            int b = next[a];
            int c = next[b];

            result.Add(a);
            result.Add(b);
            result.Add(c);
        }

        return result;
    }

    private static float SignedArea(List<Vector2> poly)
    {
        double area = 0.0;
        
        for (int i = 0; i < poly.Count; i++)
        {
            var p0 = poly[i];
            var p1 = poly[(i + 1) % poly.Count];
            area += (p0.X * p1.Y) - (p1.X * p0.Y);
        }

        return (float)(area * 0.5);
    }

    private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c) 
        => (((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X))) > 0;

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        // barycentric technique
        var v0 = new Vector2(c.X - a.X, c.Y - a.Y);
        var v1 = new Vector2(b.X - a.X, b.Y - a.Y);
        var v2 = new Vector2(p.X - a.X, p.Y - a.Y);

        var dot00 = (v0.X * v0.X) + (v0.Y * v0.Y);
        var dot01 = (v0.X * v1.X) + (v0.Y * v1.Y);
        var dot02 = (v0.X * v2.X) + (v0.Y * v2.Y);
        var dot11 = (v1.X * v1.X) + (v1.Y * v1.Y);
        var dot12 = (v1.X * v2.X) + (v1.Y * v2.Y);

        var denom = (dot00 * dot11) - (dot01 * dot01);
        if (System.Math.Abs(denom) < 1e-8)
            return false;

        var u = ((dot11 * dot02) - (dot01 * dot12)) / denom;
        var v = ((dot00 * dot12) - (dot01 * dot02)) / denom;
        return u >= 0 && v >= 0 && u + v < 1;
    }
}
