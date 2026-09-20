using SharpEngine.Core.Defaults;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace SharpEngine.Text.Fonts;

/// <summary>
///     Simple font manager responsible for scanning a fonts directory and providing Font objects.
///     Detailed glyph parsing and metrics are handled by later milestones.
/// </summary>
public class FontManager
{
    // TODO: Replace through dependency injection.
    public static FontManager Instance { get; } = new FontManager();
    private readonly Dictionary<string, Font> _fontCache = new(StringComparer.OrdinalIgnoreCase);

    private readonly IFileSystem _fileSystem;

    private FontManager(IFileSystem? fileSystem = null)
    {
        _fileSystem = fileSystem ?? new FileSystem();

        // Attempt to pre-load fonts from default directory if present
        try
        {
            if (_fileSystem.Directory.Exists(Defaults.FontsDirectory))
                LoadFontsFromDirectory(Defaults.FontsDirectory);
        }
        catch
        {
            // TODO: swallow errors for now; more explicit errors/diagnostics will be added in future milestones
        }
    }

    /// <summary>
    ///     Loads all font files found in the specified directory.
    ///     Supported extensions: .ttf, .otf
    /// </summary>
    /// <param name="directory">Directory to scan for font files.</param>
    public void LoadFontsFromDirectory(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentNullException(nameof(directory));

        if (!_fileSystem.Directory.Exists(directory))
            return;

        var files = _fileSystem.Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
            .Where(f => f.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".otf", StringComparison.OrdinalIgnoreCase));

        foreach (var file in files)
        {
            try
            {
                var font = new Font(file);
                _fontCache[font.Name] = font;
            }
            catch
            {
                // TODO: Handle invalid font files
            }
        }
    }

    /// <summary>
    ///     Loads a single font file and returns the Font object.
    /// </summary>
    public Font LoadFont(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!_fileSystem.File.Exists(filePath))
            throw new FileNotFoundException("Font file not found", filePath);

        var font = new Font(filePath);
        _fontCache[font.Name] = font;
        return font;
    }

    /// <summary>
    ///     Attempts to get a loaded font by name.
    /// </summary>
    public bool TryGetFont(string name, out Font? font)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            font = null;
            return false;
        }

        // Try exact key first
        if (_fontCache.TryGetValue(name, out font))
            return true;

        // Fallback: try match by family name
        font = _fontCache.Values.FirstOrDefault(f => string.Equals(f.FamilyName, name, StringComparison.OrdinalIgnoreCase));
        return font != null;
    }

    /// <summary>
    ///     Gets all available font names (file-based keys).
    /// </summary>
    public IReadOnlyCollection<string> AvailableFonts => _fontCache.Keys.ToList().AsReadOnly();
}
