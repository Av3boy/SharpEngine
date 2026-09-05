using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Windowing;
using SharpEngine.Core.Entities.UI;

using Silk.NET.OpenGL;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

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

            // Render the text into a System.Drawing bitmap
            using var bmp = RenderTextToBitmap(Text, FontFamily, (int)FontSize, out var measuredWidth, out var measuredHeight);
            if (bmp is null)
                return;

            // Convert bitmap to RGBA bytes
            var rgba = BitmapToRgba(bmp);

            // Create a runtime texture from the bytes
            var tex = new Components.Properties.Textures.Texture(gl, rgba, bmp.Width, bmp.Height, path: $"<text:{Name}>");

            // Assign texture to material
            var meshRenderer = Components.OfType<MeshRenderer>().FirstOrDefault();
            if (meshRenderer is not null)
            {
                meshRenderer.Material.DiffuseMap.Texture = tex;
                meshRenderer.Material.DiffuseMap.Path = tex.Path;
            }

            // Update element dimensions to match measured size (in UI units)
            Width = measuredWidth;
            Height = measuredHeight;

            _textureCreated = true;
        }
        catch
        {
            // swallow for now
        }
    }

    public override Task Render(CameraView camera, Window window)
    {
        // Defer to UIElement.Render which draws the textured quad
        return base.Render(camera, window);
    }

    private static Bitmap RenderTextToBitmap(string text, string fontFamily, int fontSize, out int width, out int height)
    {
        if (string.IsNullOrEmpty(text))
        {
            width = 1;
            height = 1;
            return new Bitmap(1, 1);
        }

        using var tmp = new Bitmap(1, 1);
        using var gtmp = Graphics.FromImage(tmp);
        var font = new Font(fontFamily, fontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
        var size = gtmp.MeasureString(text, font);
        width = System.Math.Max(1, (int)System.Math.Ceiling(size.Width));
        height = System.Math.Max(1, (int)System.Math.Ceiling(size.Height));

        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.Clear(System.Drawing.Color.Transparent);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        using var brush = new SolidBrush(System.Drawing.Color.FromArgb((int)(255 * 1.0f), System.Drawing.Color.White));
        g.DrawString(text, font, brush, 0, 0);
        g.Flush();

        return bmp;
    }

    private static byte[] BitmapToRgba(Bitmap bmp)
    {
        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        try
        {
            var length = System.Math.Abs(data.Stride) * bmp.Height;
            var bytes = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, length);

            // Convert ARGB -> RGBA
            for (int i = 0; i < bytes.Length; i += 4)
            {
                var a = bytes[i + 3];
                var r = bytes[i + 2];
                var g = bytes[i + 1];
                var b = bytes[i + 0];

                bytes[i + 0] = r;
                bytes[i + 1] = g;
                bytes[i + 2] = b;
                bytes[i + 3] = a;
            }

            // Bitmaps are stored top-to-bottom. OpenGL expects pixel data with the origin at the lower-left.
            // Flip the image rows vertically so the uploaded texture appears right-side-up.
            var stride = System.Math.Abs(data.Stride);
            var flipped = new byte[bytes.Length];
            var height = bmp.Height;
            for (int row = 0; row < height; row++)
            {
                var srcOffset = row * stride;
                var dstOffset = (height - 1 - row) * stride;
                System.Buffer.BlockCopy(bytes, srcOffset, flipped, dstOffset, stride);
            }

            return flipped;
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }
}
