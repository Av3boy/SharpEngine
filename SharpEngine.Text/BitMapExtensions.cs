using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SharpEngine.Core.Rendering;

public enum Alpha
{
    Transparent = 0,
    Opague = 255
}

/// <summary>
/// Windows-only text bitmap renderer. Isolated into its own file so the containing callers
/// can remain platform-agnostic and only call these APIs behind an OS check.
/// </summary>
[SupportedOSPlatform("windows")]
public static class BitMapExtensions
{
    // TODO: Apply the adapter pattern to the bitmaps generated here so that they become more testable.

    public static Bitmap RenderTextToBitmap(string text, string fontFamily, int fontSize, out int width, out int height)
    {
        if (string.IsNullOrEmpty(text))
        {
            width = 1;
            height = 1;
            return new Bitmap(width, height);
        }

        var font = new System.Drawing.Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        GetBitMapSize(font, text, out width, out height);

        var bmp = new Bitmap(width, height);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            
            using var brush = new SolidBrush(Color.FromArgb(255, Color.White));
            g.DrawString(text, font, brush, 0, 0);
            g.Flush();
        }

        // Trim transparent borders to reduce texture size
        try
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = System.Math.Abs(data.Stride);
            int bytesLen = stride * bmp.Height;
            var bytes = new byte[bytesLen];

            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, bytesLen);
            bmp.UnlockBits(data);

            int left = bmp.Width, top = bmp.Height, right = 0, bottom = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                int rowStart = y * stride;
                for (int x = 0; x < bmp.Width; x++)
                {
                    int idx = rowStart + (x * 4);
                    byte a = bytes[idx + 3]; // ARGB layout when locked

                    // threshold to ignore near-transparent antialiasing pixels
                    if (a <= 8)
                        continue;
                    
                    if (x < left)
                        left = x;

                    if (x > right)
                        right = x;

                    if (y < top)
                        top = y;

                    if (y > bottom)
                        bottom = y;
                }
            }

            if (right < left || bottom < top)
            {
                // Fully transparent — return a 1x1 transparent bitmap
                width = 1;
                height = 1;
                return new Bitmap(width, height);
            }

            var crop = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
            var cropped = bmp.Clone(crop, PixelFormat.Format32bppArgb);

            width = cropped.Width;
            height = cropped.Height;

            bmp.Dispose();
            return cropped;
        }
        catch
        {
            // TODO: Should we just re-throw here instead and in "finally" dispose?
            // On any failure during trim, return the original full bitmap.
            width = bmp.Width;
            height = bmp.Height;
            return bmp;
        }
    }

    private static void GetBitMapSize(System.Drawing.Font font, string text, out int width, out int height) 
    {
        using var tmp = new Bitmap(1, 1);
        using var gtmp = Graphics.FromImage(tmp);

        var size = gtmp.MeasureString(text, font);

        width = System.Math.Max(1, (int)System.MathF.Ceiling(size.Width));
        height = System.Math.Max(1, (int)System.MathF.Ceiling(size.Height));
    }

    public static byte[] BitmapToRgba(Bitmap bmp)
    {
        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

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

            // Return RGBA bytes (no vertical flip here — Texture runtime handles row orientation).
            return bytes;
        }
        catch 
        {
            // Let all exceptions bubble up so they can be logged or otherwise properly prosessed.
            throw;
        }
        finally
        {
            // Once the operation succeeds or an exception has been catched, make sure that the data is unlocked.
            bmp.UnlockBits(data);
        }
    }
}
