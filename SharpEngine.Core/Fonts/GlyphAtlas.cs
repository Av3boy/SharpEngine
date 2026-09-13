using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Fonts;

/// <summary>
/// Simple glyph atlas implementation. Packs bitmaps into a single texture and exposes UV rectangles per glyph.
/// For now uses a naive skyline packing algorithm and stores glyph bitmaps as byte[] grayscale rows.
/// SDF generation is provided as a simple naive distance transform for prototyping; future versions will use multi-channel SDF and optimized kernels.
/// </summary>
public class GlyphAtlas : IDisposable
{
    private readonly Dictionary<string, Rectangle> _uvRects = new();
    private readonly Dictionary<string, byte[]> _glyphBitmaps = new();

    public int Width { get; private set; }
    public int Height { get; private set; }

    public GlyphAtlas(int width = 1024, int height = 1024)
    {
        Width = width;
        Height = height;
    }

    public void AddGlyphBitmap(string glyphName, int w, int h, byte[] pixels)
    {
        if (pixels is null)
            throw new ArgumentNullException(nameof(pixels));
        
        if (pixels.Length != w * h)
            throw new ArgumentException("Pixel length mismatch");

        _glyphBitmaps[glyphName] = pixels;
    }

    /// <summary>
    /// Packs the currently added glyph bitmaps into the atlas texture using a simple row-based packer.
    /// </summary>
    public void Pack()
    {
        // naive packing: place glyphs in rows until full
        var x = 0;
        var y = 0;
        var rowHeight = 0;

        foreach (var kv in _glyphBitmaps)
        {
            var name = kv.Key;
            // For prototype we assume glyph metadata contains width/height encoded in name as name|w|h
            var parts = name.Split('|');
            var w = int.Parse(parts[1]);
            var h = int.Parse(parts[2]);

            if (x + w > Width)
            {
                x = 0;
                y += rowHeight;
                rowHeight = 0;
            }

            if (y + h > Height)
                throw new InvalidOperationException("Atlas too small for glyph set");

            _uvRects[kv.Key] = new Rectangle(x, y, w, h);
            x += w;
            rowHeight = System.Math.Max(rowHeight, h);
        }
    }

    public bool TryGetUV(string glyphName, out Rectangle rect)
    {
        return _uvRects.TryGetValue(glyphName, out rect);
    }

    /// <summary>
    /// Generates a simple (slow) SDF for the glyph bitmap using brute-force distance transform.
    /// Returns float array with signed distances normalized to 0..1 map with 0.5 as zero distance.
    /// </summary>
    public static float[] GenerateSDF(int w, int h, byte[] bitmap, int spread = 16)
    {
        var sdf = new float[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var idx = y * w + x;
                var inside = bitmap[idx] > 127;
                var bestDistSq = int.MaxValue;

                for (int yy = 0; yy < h; yy++)
                {
                    for (int xx = 0; xx < w; xx++)
                    {
                        var idx2 = yy * w + xx;
                        var inside2 = bitmap[idx2] > 127;
                        if (inside == inside2)
                            continue;

                        var dx = xx - x;
                        var dy = yy - y;
                        var dsq = dx * dx + dy * dy;
                        
                        if (dsq < bestDistSq)
                            bestDistSq = dsq;
                    }
                }

                var dist = System.Math.Sqrt(bestDistSq);
                // positive outside distances, negative inside distances
                sdf[idx] = (float)(dist / spread / 2.0 + 0.5) * (inside ? -1 : 1);
            }
        }

        return sdf;
    }

    public void Dispose()
    {
        _uvRects.Clear();
        _glyphBitmaps.Clear();
    }
}
