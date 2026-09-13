using SharpEngine.Core.Numerics;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Versioning;

namespace SharpEngine.Core.Fonts;

/// <summary>
///     Extracts glyph contours from a text string using GDI+ GraphicsPath.
/// </summary>
/// <remarks>
///     This is a pragmatic approach for extracting vector outlines from installed fonts or local font files (via PrivateFontCollection).
///     It flattens curves to line segments and returns contours as lists of Vector2 points in pixel space.
/// </remarks>
public class FontGlyphExtractor
{
    private readonly TypographyGlyphExtractor _typographyGlyphExtractor;

    /// <summary>
    ///     Initializes a new instance of <see cref="FontGlyphExtractor" />.
    /// </summary>
    public FontGlyphExtractor(TypographyGlyphExtractor typographyGlyphExtractor)
    {
        _typographyGlyphExtractor = typographyGlyphExtractor;
    }

    [SupportedOSPlatform("windows")]
    public Glyph ExtractGlyph(string text, string fontFamily, float fontSize, float dpi = 96f)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentNullException(nameof(text));

        // If a Typography-based extractor is available, attempt to use it first (managed integration via reflection). Fallback to PrivateFontCollection GDI+ approach when unavailable.
        bool fontFound = FontManager.Instance.TryGetFont(fontFamily, out var font);
        if (fontFound && font is not null)
        {
            if (_typographyGlyphExtractor.TryExtractGlyph(font.FilePath, text, fontSize, out var ttGlyph, out var _))
                return ttGlyph;
        }

        var fontFamToUse = GetFontFamilyToUse(fontFamily, fontFound, font);
        GetPathData(fontFamToUse, text, fontSize, dpi, out var pts, out var types);

        var glyph = new Glyph(text);
        var currentPoints = new List<Vector2>();

        for (int i = 0; i < pts.Length; i++)
        {
            var p = pts[i];
            var t = types[i];

            var v = new Vector2(p.X, p.Y);
            currentPoints.Add(v);

            // If the point type has CloseSubpath flag, finalize the contour
            bool isClose = (t & (byte)PathPointType.CloseSubpath) != 0;
            bool isLast = i == pts.Length - 1;

            if (!isClose && !isLast)
                continue;

            if (currentPoints.Count >= 2)
            {
                // Convert polyline into GlyphSegments as lines
                var segments = new List<GlyphSegment>();
                for (int j = 0; j < currentPoints.Count - 1; j++)
                {
                    segments.Add(new GlyphSegment(SegmentType.Line, currentPoints[j], currentPoints[j + 1]));
                }

                glyph.AddContour(segments);
            }

            currentPoints.Clear();
        }

        return glyph;
    }

    [SupportedOSPlatform("windows")]
    private static FontFamily GetFontFamilyToUse(string fontFamily, bool fontFound, Font fontFile)
    {
        if (fontFound)
        {
            using var pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontFile.FilePath);
            return pfc.Families.Length > 0 ? pfc.Families[0] : new FontFamily(fontFamily);
        }
        
        return new FontFamily(fontFamily);
    }

    [SupportedOSPlatform("windows")]
    private static void GetPathData(FontFamily fam, string text, float fontSize, float dpi, out PointF[] pts, out byte[] types)
    {
        // Create a GraphicsPath with the text outlines
        using var path = new GraphicsPath();
        path.AddString(text, fam, (int)FontStyle.Regular, fontSize * (dpi / 72f), PointF.Empty, StringFormat.GenericDefault);

        // Flatten the path to convert bezier curves to line segments
        path.Flatten();

        pts = path.PathPoints;
        types = path.PathTypes;
    }
}
