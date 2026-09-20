using Silk.NET.OpenGL;

namespace SharpEngine.Core.Fonts;

public static class GlyphAtlasExtensions
{
    /// <summary>
    /// Builds a runtime RGBA texture for the atlas and returns a Texture instance.
    /// Glyph bitmaps are assumed to be single-channel grayscale stored in _glyphBitmaps entries.
    /// </summary>
    public static SharpEngine.Core.Components.Properties.Textures.Texture BuildRuntimeTexture(this GlyphAtlas atlas, GL gl)
    {
        // Create RGBA buffer
        var rgba = new byte[atlas.Width * atlas.Height * 4];

        foreach (var kv in atlas.GetGlyphEntries())
        {
            var key = kv.key;
            var rect = kv.rect;
            var pixels = kv.pixels; // grayscale length == rect.Width*rect.Height

            for (int y = 0; y < rect.Height; y++)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    int srcIdx = y * rect.Width + x;
                    int dstIdx = ((rect.Y + y) * atlas.Width + (rect.X + x)) * 4;
                    byte v = pixels[srcIdx];
                    rgba[dstIdx + 0] = v;
                    rgba[dstIdx + 1] = v;
                    rgba[dstIdx + 2] = v;
                    rgba[dstIdx + 3] = 255;
                }
            }
        }

        var tex = new SharpEngine.Core.Components.Properties.Textures.Texture(gl, rgba, atlas.Width, atlas.Height, path: "<glyph-atlas>");
        return tex;
    }

    private static System.Collections.Generic.IEnumerable<(string key, System.Drawing.Rectangle rect, byte[] pixels)> GetGlyphEntries(this GlyphAtlas atlas)
    {
        // Reflection-ish access to private fields (avoid changing GlyphAtlas public API in this iteration)
        var t = atlas.GetType();
        var uvField = t.GetField("_uvRects", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bmpField = t.GetField("_glyphBitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var uv = (System.Collections.IDictionary)uvField.GetValue(atlas);
        var bmp = (System.Collections.IDictionary)bmpField.GetValue(atlas);

        var keys = new System.Collections.Generic.List<object>();
        foreach (System.Collections.DictionaryEntry de in bmp) 
            keys.Add(de.Key);

        foreach (var k in keys)
        {
            var name = (string)k;
            var rect = (System.Drawing.Rectangle)uv[name];
            var pixels = (byte[])bmp[name];
            yield return (name, rect, pixels);
        }
    }
}
