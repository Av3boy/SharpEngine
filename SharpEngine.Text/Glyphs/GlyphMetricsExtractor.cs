using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Numerics;

namespace SharpEngine.Text.Glyphs;

/// <summary>
///     Pragmatic glyph extractor and metrics provider using GDI+ PrivateFontCollection.
///     Provides per-character outlines (flattened) and advance widths and pairwise kerning (measured).
///     This is not a full shaping engine (no HarfBuzz) but gives reasonable metrics for Latin scripts.
/// </summary>
internal static class GlyphMetricsExtractor
{
    private static readonly ConcurrentDictionary<string, (Glyph glyph, float advance)> _glyphCache = new();
    private static readonly ConcurrentDictionary<string, float> _kerningCache = new();

    [SupportedOSPlatform("windows")]
    public static (Glyph glyph, float advance) GetGlyphAndAdvance(string fontFilePath, string familyName, char ch, float fontSize, float dpi = 96f)
    {
        var key = $"{fontFilePath ?? "<system>"}|{familyName}|{ch}|{fontSize}";
        if (_glyphCache.TryGetValue(key, out var v))
            return v;

        Glyph glyph;
        float advance = 0f;

        using var bmp = new Bitmap(1, 1);
        using var g = Graphics.FromImage(bmp);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        var fam = GetFontFamily(fontFilePath, familyName);

        using var sysFont = new System.Drawing.Font(fam, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        // Build GraphicsPath outline for the single character
        using var path = new GraphicsPath();
        path.AddString(ch.ToString(), fam, (int)FontStyle.Regular, fontSize * (dpi / 72f), PointF.Empty, StringFormat.GenericDefault);
        path.Flatten();

        glyph = new Glyph(ch.ToString());
        var pts = path.PathPoints;
        var types = path.PathTypes;

        var currentPoints = new List<Vector2>();
        for (int i = 0; i < pts.Length; i++)
        {
            var p = pts[i];
            var t = types[i];

            currentPoints.Add(new Vector2(p.X, p.Y));

            bool isClose = (t & (byte)PathPointType.CloseSubpath) != 0;
            bool isLast = i == pts.Length - 1;

            if (isClose || isLast)
            {
                if (currentPoints.Count >= 2)
                {
                    var segments = new List<GlyphSegment>();
                    for (int j = 0; j < currentPoints.Count - 1; j++)
                    {
                        segments.Add(new GlyphSegment(SegmentType.Line, currentPoints[j], currentPoints[j + 1]));
                    }

                    glyph.AddContour(segments);
                }

                currentPoints.Clear();
            }
        }

        // Measure advance using MeasureCharacterRanges
        var layout = new RectangleF(0, 0, 1000, 1000);
        var sf = new StringFormat();
        sf.SetMeasurableCharacterRanges([new CharacterRange(0, 1)]);
        var ranges = g.MeasureCharacterRanges(ch.ToString(), sysFont, layout, sf);
        if (ranges != null && ranges.Length > 0)
        {
            var rect = ranges[0].GetBounds(g);
            advance = rect.Width;
        }
        else
        {
            var size = g.MeasureString(ch.ToString(), sysFont);
            advance = size.Width;
        }

        _glyphCache[key] = (glyph, advance);
        return (glyph, advance);
    }

    [SupportedOSPlatform("windows")]
    private static FontFamily GetFontFamily(string fontFilePath, string familyName)
    {
        // Create font
        FontFamily? fam = null;
        if (!string.IsNullOrEmpty(fontFilePath) && System.IO.File.Exists(fontFilePath))
        {
            try
            {
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile(fontFilePath);
                if (pfc.Families.Length > 0)
                    fam = pfc.Families[0];
            }
            catch
            {
                fam = null;
            }
        }

        if (fam is null)
        {
            try
            {
                fam = new FontFamily(familyName);
            }
            catch
            {
                fam = FontFamily.GenericSansSerif;
            }
        }

        return fam ?? FontFamily.GenericSansSerif;
    }

    [SupportedOSPlatform("windows")]
    public static float GetKerning(string fontFilePath, string familyName, char left, char right, float fontSize)
    {
        var key = $"kern|{fontFilePath ?? "<system>"}|{familyName}|{left}|{right}|{fontSize}";
        if (_kerningCache.TryGetValue(key, out var v)) 
            return v;

        using var bmp = new Bitmap(1,1);
        using var g = Graphics.FromImage(bmp);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        FontFamily fam = GetFontFamily(fontFilePath, familyName);

        using var sysFont = new System.Drawing.Font(fam, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        // Measure pair width
        var layout = new RectangleF(0, 0, 2000, 2000);
        var sizePair = g.MeasureString(new string([left, right]), sysFont);
        var sizeLeft = g.MeasureString(left.ToString(), sysFont);
        var sizeRight = g.MeasureString(right.ToString(), sysFont);

        var kern = sizePair.Width - (sizeLeft.Width + sizeRight.Width);
        _kerningCache[key] = kern;
        return kern;
    }
}
