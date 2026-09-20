using System.Collections.Concurrent;
using SharpEngine.Core.Components.Properties.Textures;

namespace SharpEngine.Text;

// Simple runtime cache for text rasterized textures keyed by text/font/size.
internal static class TextTextureCache
{
    private static readonly ConcurrentDictionary<string, (Texture tex, int w, int h)> _cache = new();

    public static bool TryGet(string key, out Texture? tex, out int w, out int h)
    {
        if (_cache.TryGetValue(key, out var v))
        {
            tex = v.tex;
            w = v.w;
            h = v.h;
            return true;
        }

        tex = null;
        w = 0; 
        h = 0;

        return false;
    }

    public static Texture GetOrCreate(Silk.NET.OpenGL.GL gl, string key, byte[] rgba, int width, int height, string? path = null)
    {
        // fast path
        if (_cache.TryGetValue(key, out var existing))
            return existing.tex;

        var tex = new Texture(gl, rgba, width, height, path: path ?? $"<text:{key}>");
        _cache[key] = (tex, width, height);
        return tex;
    }

    public static void Clear()
    {
        foreach (var kv in _cache)
        {
            try 
            { 
            kv.Value.tex?.Dispose(); 
            } 
            catch { }
        }

        _cache.Clear();
    }
}
