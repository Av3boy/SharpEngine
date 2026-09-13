using Microsoft.Extensions.Logging;
using System;
using System.IO.Abstractions;

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

    /// <summary>Primary family name parsed from the font file when available.</summary>
    public string FamilyName { get; }

    /// <summary>
    ///     Raw font bytes. Loaded lazily on first access.
    /// </summary>
    public byte[] Data
    {
        get
        {
            field ??= _fileSystem.File.ReadAllBytes(FilePath);
            return field;
        }
    }

    private readonly IFileSystem _fileSystem;
    private readonly ILogger<Font> _logger;

    public Font(string filePath) : this(filePath, new FileSystem(), new LoggerFactory().CreateLogger<Font>()) { }

    public Font(string filePath, IFileSystem fileSystem, ILogger<Font> logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        Name = _fileSystem.Path.GetFileNameWithoutExtension(filePath);
        FilePath = filePath;

        // TODO: Replace with a cross-platform font parsing library to extract the family name from the font file.

        // Attempt to extract family name using PrivateFontCollection so we can map by family
        using var pfc = new System.Drawing.Text.PrivateFontCollection();
        pfc.AddFontFile(filePath);

        // TODO: Can there be multiple families in a single font file? If so, we may need to handle that differently.
        if (pfc.Families.Length > 0)
            FamilyName = pfc.Families[0].Name;
        else
            FamilyName = Name;
    }
}
