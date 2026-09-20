using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Windowing;

using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System;
using SharpEngine.Core.Rendering;
using SharpEngine.Text.Fonts;
using SharpEngine.Text.Glyphs;
using SharpEngine.Text;
using SharpEngine.Core.UI.Entities;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a text element that can be placed in the scene.
///     Uses UIElement as a base so it can reuse the UI mesh/shader for textured quads.
///     This implementation draws the text into a bitmap and uploads it as a texture on initialization.
/// </summary>
public class TextElement : UIElement
{
    /// <summary>Gets or sets the text content.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the font family name.</summary>
    public string FontFamily { get; set; } = "Segoe UI";

    /// <summary>Gets or sets the font size in points.</summary>
    public float FontSize { get; set; } = 16.0f;

    /// <summary>Gets or sets the color of the text.</summary>
    public Vector4 Color { get; set; } = new Vector4(1, 1, 1, 1);

    private bool _textureCreated = false;

    /// <summary>
    /// When true, attempt to render the glyph as vector geometry (triangulated) rather than rasterizing to a bitmap.
    /// </summary>
    public bool UseVectorRendering { get; set; } = true;

    private readonly ILogger? _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="TextElement"/>.
    /// </summary>
    /// <param name="textContent">The text content.</param>
    public TextElement(string textContent) : this(textContent, "TextElement") { }

    /// <summary>
    ///    Initializes a new instance of <see cref="TextElement"/>.
    /// </summary>
    /// <param name="textContent">The text content.</param>
    /// <param name="name">The name of the text element.</param>
    public TextElement(string textContent, string name) : base(name) 
    {
        Text = textContent;
    }

    /// <inheritdoc />
    public override void OnInitialized(GL gl)
    {
        base.OnInitialized(gl);

        try
        {
            if (_textureCreated)
                return;

            if (UseVectorRendering)
            {
                RenderVector(gl);
            }

            // Rasterization fallback: only use System.Drawing on Windows where it's supported.
            if (!OperatingSystem.IsWindows())
            {
                // On non-Windows platforms, skip the System.Drawing raster path.
                // The vector path or other rendering approaches (SDF, external renderer) should be used instead.
                return;
            }

            using var bmp = BitMapExtensions.RenderTextToBitmap(Text, FontFamily, (int)FontSize, out var measuredWidth, out var measuredHeight);
            if (bmp is null)
                return;

            var rgba = BitMapExtensions.BitmapToRgba(bmp);
            var key = GetTextTextureCacheKey();
            var tex = TextTextureCache.GetOrCreate(gl, key, rgba, bmp.Width, bmp.Height, path: $"<text:{Name}>");

            var meshRenderer = Components.OfType<MeshRenderer>().FirstOrDefault();
            if (meshRenderer is not null)
            {
                meshRenderer.Material.DiffuseMap.Texture = tex;
                meshRenderer.Material.DiffuseMap.Path = tex.Path;
            }

            Width = measuredWidth;
            Height = measuredHeight;

            _textureCreated = true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing text element: {ex}", ex);
        }
    }

    private void RenderVector(GL gl)
    {
        try
        {
            // Layout per-glyph using metrics extractor and triangulate each glyph into a single mesh.
            float penX = 0f;
            var allVerts = new List<float>();
            var allIndices = new List<uint>();
            var vertCount = 0u;

            // Determine font file path if available
            Font? fontFile = null;
            if (FontManager.Instance.TryGetFont(FontFamily, out var f))
                fontFile = f;

            for (int i = 0; i < Text.Length; i++)
            {
                var ch = Text[i];
                var glyphKey = $"{fontFile?.FilePath ?? "<system>"}|{FontFamily}|{ch}|{FontSize}";

                if (!GlyphMeshCache.TryGet(glyphKey, out var cachedMesh, out var cachedAdvance, out var gwidth, out var gheight))
                {
                    var (glyph, advance) = GlyphMetricsExtractor.GetGlyphAndAdvance(fontFile?.FilePath, FontFamily, ch, FontSize);
                    var tris = glyph.Triangulate().ToList();
                    if (tris.Count < 3)
                        continue;

                    // Build vertices for this glyph and store as cache
                    var verts = new List<float>(tris.Count * 8);
                    var indices = new List<uint>(tris.Count);
                    for (int t = 0; t < tris.Count; t++)
                    {
                        var v = tris[t];
                        verts.Add(v.X);
                        verts.Add(v.Y);
                        verts.Add(0f);

                        verts.Add(0f); 
                        verts.Add(0f); 
                        verts.Add(1f);
                        
                        verts.Add(0f); 
                        verts.Add(0f);
                        
                        indices.Add((uint)t);
                    }

                    var mesh = GlyphMeshCache.GetOrCreate(gl, glyphKey, verts.ToArray(), indices.ToArray(), advance, 0, 0);
                    cachedAdvance = advance;
                }

                // Kerning with next char
                float kern = 0f;
                if (i + 1 < Text.Length)
                {
                    kern = GlyphMetricsExtractor.GetKerning(fontFile?.FilePath, FontFamily, ch, Text[i + 1], FontSize);
                }

                // Fetch cached mesh vertices by re-obtaining the mesh (we stored verts earlier in cache as mesh only)
                if (GlyphMeshCache.TryGet(glyphKey, out var meshObj, out var adv, out var w, out var h))
                {
                    // Read mesh vertices by reflecting internal Mesh buffers is complex; instead, regenerate triangles for placement.
                    var (glyph2, advance2) = GlyphMetricsExtractor.GetGlyphAndAdvance(fontFile?.FilePath, FontFamily, ch, FontSize);
                    var tris2 = glyph2.Triangulate().ToList();
                    for (int t = 0; t < tris2.Count; t++)
                    {
                        var vx = tris2[t].X + penX;
                        var vy = tris2[t].Y;

                        allVerts.Add(vx);
                        allVerts.Add(vy);
                        allVerts.Add(0f);

                        allVerts.Add(0f);
                        allVerts.Add(0f);
                        allVerts.Add(1f);

                        allVerts.Add(0f);
                        allVerts.Add(0f);

                        allIndices.Add(vertCount++);
                    }

                    penX += adv + kern;
                }
            }

            // Evil Artifact Detector: if the total triangle count is excessive or font size is very large, fallback to bitmap/SDF path.
            var triCount = allIndices.Count / 3;
            if (triCount > 2000 || FontSize > 128)
            {
                // Abort vector path and let bitmap fallback handle this element for better performance and memory usage.
                return;
            }

            if (allVerts.Count >= 8 && allIndices.Count >= 3)
            {
                var mesh = new Properties.Meshes.Mesh(gl, allVerts.ToArray(), allIndices.ToArray());
                var white = new byte[] { 255, 255, 255, 255 };
                var whiteTex = new Components.Properties.Textures.Texture(gl, white, 1, 1, path: $"<white:{Name}>");
                var mat = new Components.Properties.Material($"text-vector-{Name}", whiteTex);

                _ = Components.RemoveAll(c => c is MeshRenderer);
                Components.Add(new MeshRenderer(mesh, mat));

                // Width is penX
                Width = penX;
                Height = FontSize;

                _textureCreated = true;
                return;
            }
        }
        catch (Exception ex)
        {
            // On any failure, fall through to bitmap raster fallback
            _logger?.LogError(ex, "Error initializing text element: {ex}", ex);
        }
    }

    private string GetTextTextureCacheKey()
    {
        var x = Color.X.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var y = Color.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var z = Color.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var w = Color.W.ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        return $"{Text}|{FontFamily}|{FontSize}|{x}|{y}|{z}|{w}";
    }

    // Defer to UIElement.Render which draws the textured quad
    /// <inheritdoc />
    public override Task Render(CameraView camera, Window window)
        => base.Render(camera, window);
}
