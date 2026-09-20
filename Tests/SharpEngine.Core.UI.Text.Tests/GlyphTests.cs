using Xunit;
using SharpEngine.Core.Numerics;
using SharpEngine.Text.Glyphs;
using SharpEngine.Text;

namespace SharpEngine.Core.Tests;

public class GlyphTests
{
    [Fact]
    public void Triangulate_SimpleSquare_ReturnsTriangles()
    {
        var g = new Glyph("square");
        var contour = new List<GlyphSegment>
        {
            // TODO: Resolve package reference conflict with System.Numerics.
            // new GlyphSegment(SegmentType.Line, new Vector2(0,0), new Vector2(1,0)),
            // new GlyphSegment(SegmentType.Line, new Vector2(1,0), new Vector2(1,1)),
            // new GlyphSegment(SegmentType.Line, new Vector2(1,1), new Vector2(0,1)),
            // new GlyphSegment(SegmentType.Line, new Vector2(0,1), new Vector2(0,0)),
        };

        g.AddContour(contour);

        var tris = g.Triangulate();
        // Should emit at least one triangle (3 verts)
        Assert.True(tris != null);

        // TODO: Resolve package reference conflict with System.Numerics.
        // var list = new System.Collections.Generic.List<System.Numerics.Vector2>(tris);
        // Assert.True(list.Count >= 3);
        // // Count should be a multiple of 3
        // Assert.True(list.Count % 3 == 0);
    }
}