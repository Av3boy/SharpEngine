using SharpEngine.Core.Entities.Properties.Meshes;
using Silk.NET.OpenGL;
using System.Collections.Concurrent;

namespace SharpEngine.Text.Glyphs;

internal static class GlyphMeshCache
{
    private static readonly ConcurrentDictionary<string, (Mesh mesh, float advance, float width, float height)> _cache = new();

    public static bool TryGet(string key, out Mesh? mesh, out float advance, out float width, out float height)
    {
        if (_cache.TryGetValue(key, out var v))
        {
            mesh = v.mesh; 
            advance = v.advance; 
            width = v.width; 
            height = v.height; 
            
            return true;
        }

        mesh = null; 
        advance = 0; 
        width = 0; 
        height = 0; 
        
        return false;
    }

    public static Mesh GetOrCreate(GL gl, string key, float[] verts, uint[] indices, float advance, float width, float height)
    {
        if (_cache.TryGetValue(key, out var v)) 
            return v.mesh;

        var mesh = new Mesh(gl, verts, indices);
        _cache[key] = (mesh, advance, width, height);

        return mesh;
    }
}
