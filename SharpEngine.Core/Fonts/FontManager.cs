using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpEngine.Core.Fonts;

/// <summary>
///     Simple font manager responsible for scanning a fonts directory and providing Font objects.
///     Detailed glyph parsing and metrics are handled by later milestones.
/// </summary>
public class FontManager
{
    public static FontManager Instance { get; } = new FontManager();

    private readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>(StringComparer.OrdinalIgnoreCase);

    private FontManager()
    {
        // Attempt to pre-load fonts from default directory if present
        try
        {
            if (Directory.Exists(_Resources.Default.FontsDirectory))
                LoadFontsFromDirectory(_Resources.Default.FontsDirectory);
        }
        catch
        {
            // swallow errors for now; more explicit errors/diagnostics will be added in future milestones
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

        if (!Directory.Exists(directory))
            return;

        var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
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
                // skip invalid font files
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

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Font file not found", filePath);

        var font = new Font(filePath);
        _fontCache[font.Name] = font;
        return font;
    }

    /// <summary>
    ///     Attempts to get a loaded font by name.
    /// </summary>
    public bool TryGetFont(string name, out Font font)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            font = null!;
            return false;
        }

        return _fontCache.TryGetValue(name, out font);
    }

    /// <summary>
    ///     Gets all available font names.
    /// </summary>
    public IReadOnlyCollection<string> AvailableFonts => _fontCache.Keys.ToList().AsReadOnly();
}
