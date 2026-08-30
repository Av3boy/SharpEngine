using System;

namespace SharpEngine.Core.Fonts;

/// <summary>
///     Lightweight representation of a font file. Detailed glyph parsing is handled by later milestones.
/// </summary>
public class Font
{
    /// <summary>Name of the font (derived from file name).</summary>
    public string Name { get; }

    /// <summary>Full file path to the font file.</summary>
    public string FilePath { get; }

    private byte[]? _data;

    /// <summary>
    ///     Raw font bytes. Loaded lazily on first access.
    /// </summary>
    public byte[] Data
    {
        get
        {
            if (_data is null)
            {
                _data = System.IO.File.ReadAllBytes(FilePath);
            }

            return _data;
        }
    }

    public Font(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        FilePath = filePath;
        Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
    }
}
