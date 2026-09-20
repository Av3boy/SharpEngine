extern alias typography;
using SharpEngine.Text.Glyphs;
using System.Collections.Generic;
using System.Numerics;

namespace SharpEngine.Text;

// Translator implementing IGlyphTranslator to convert commands into GlyphSegments.
internal sealed class Translator : typography::Typography.OpenFont.IGlyphTranslator
{
    private readonly Glyph _glyph;
    private readonly float _scale;
    private readonly List<GlyphSegment> _segments = new();
    private float _cx, _cy;

    public Translator(Glyph glyph, float fontSize, typography::Typography.OpenFont.Typeface typeface)
    {
        _glyph = glyph;
        _scale = CalculateScale(typeface, fontSize);
    }

    private static float CalculateScale(typography::Typography.OpenFont.Typeface typeface, float fontSize)
    {
        try
        {
            float dpi = 96f;
            return typeface.CalculateScaleToPixelFromPointSize(fontSize, (int)dpi);
        }
        catch
        {
            return typeface.CalculateScaleToPixel(fontSize);
        }
    }

    public float GetScale() => _scale;

    public void BeginRead(int contourCount) { }
    public void EndRead()
    {
        if (_segments.Count > 0)
        {
            _glyph.AddContour(_segments);
            _segments.Clear();
        }
    }

    public void MoveTo(float x0, float y0)
    {
        Flush();

        _cx = x0 * _scale;
        _cy = -y0 * _scale;
    }

    public void LineTo(float x1, float y1)
    {
        var p0 = new Vector2(_cx, _cy);
        var p1 = new Vector2(x1 * _scale, -y1 * _scale);

        _segments.Add(new GlyphSegment(SegmentType.Line, p0, p1));

        _cx = p1.X;
        _cy = p1.Y;
    }

    public void Curve3(float x1, float y1, float x2, float y2)
    {
        var p0 = new Vector2(_cx, _cy);
        var p1 = new Vector2(x1 * _scale, -y1 * _scale);
        var p2 = new Vector2(x2 * _scale, -y2 * _scale);

        _segments.Add(new GlyphSegment(SegmentType.Quadratic, p0, p1, p2));

        _cx = p2.X;
        _cy = p2.Y;
    }

    public void Curve4(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        var p0 = new Vector2(_cx, _cy);
        var p1 = new Vector2(x1 * _scale, -y1 * _scale);
        var p2 = new Vector2(x2 * _scale, -y2 * _scale);
        var p3 = new Vector2(x3 * _scale, -y3 * _scale);

        _segments.Add(new GlyphSegment(SegmentType.Cubic, p0, p1, p2, p3));

        _cx = p3.X;
        _cy = p3.Y;
    }

    public void CloseContour() => Flush();

    private void Flush()
    {
        if (_segments.Count > 0)
        {
            _glyph.AddContour(_segments);
            _segments.Clear();
        }
    }
}
