extern alias typography;
using System;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using SharpEngine.Text.Glyphs;

namespace SharpEngine.Text;

/// <summary>
/// Bridge to Typography.OpenFont to extract glyph outlines and advance widths.
/// The implementation is a single-file, typed integration (no reflection for advance/outline).
/// </summary>
public class TypographyGlyphExtractor
{
    private readonly ILogger? _logger;
    private readonly IFileSystem _fileSystem;

    internal TypographyGlyphExtractor(ILogger? logger = null, IFileSystem? fileSystem = null)
    {
        _logger = logger;
        _fileSystem = fileSystem ?? new FileSystem();
    }

    public bool TryExtractGlyph(string fontFilePath, string text, float fontSize, out Glyph glyph, out float advance)
    {
        glyph = null!;
        advance = 0f;

        if (string.IsNullOrEmpty(fontFilePath) || !_fileSystem.File.Exists(fontFilePath) || string.IsNullOrEmpty(text))
        {
            _logger?.LogDebug("TryExtractGlyph called with invalid arguments (file exists: {exists}, text empty: {textEmpty}).", _fileSystem.File.Exists(fontFilePath), string.IsNullOrEmpty(text));
            return false;
        }

        try
        {
            if (TryExtractWithTypography(fontFilePath, text, fontSize, out glyph, out advance, _logger))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Typography extractor threw an exception.");
        }

        _logger?.LogDebug("No glyph extraction succeeded for {file} and text '{text}'.", fontFilePath, text);
        glyph = null!;
        advance = 0f;

        return false;
    }

    // Typed implementation using Typography.OpenFont (compile-time dependency).
    private bool TryExtractWithTypography(string fontFilePath, string text, float fontSize, out Glyph glyph, out float advance, ILogger? logger = null)
    {
        glyph = null!;
        advance = 0f;

        try
        {
            var reader = new typography::Typography.OpenFont.OpenFontReader();
            typography::Typography.OpenFont.Typeface typeface;
            using var fs = _fileSystem.File.OpenRead(fontFilePath);
            typeface = reader.Read(fs);

            if (typeface == null)
            {
                logger?.LogDebug("OpenFontReader.Read returned null for {file}.", fontFilePath);
                return false;
            }

            int codepoint = char.ConvertToUtf32(text, 0);
            int glyphIndex = typeface.LookupIndex(codepoint, 0);

            var tfGlyph = typeface.GetGlyphByIndex(glyphIndex);
            if (tfGlyph == null)
            {
                logger?.LogDebug("Typeface.GetGlyphByIndex returned null for index {idx}.", glyphIndex);
                return false;
            }

            var resultGlyph = new Glyph(text);

            var translator = new Translator(resultGlyph, fontSize, typeface);
            float scale = translator.GetScale();

            var glyphPoints = tfGlyph.GlyphPoints;
            var endPoints = tfGlyph.EndPoints;

            if (glyphPoints != null && endPoints != null && glyphPoints.Length > 0 && endPoints.Length > 0)
                typography::Typography.OpenFont.IGlyphReaderExtensions.Read(translator, glyphPoints, endPoints, scale);

            advance = GetAdvanceWidth(typeface, glyphIndex, codepoint, scale);
            glyph = resultGlyph;

            return true;
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Typed typography extraction failed for {file}.", fontFilePath);
            glyph = null!;
            advance = 0f;

            return false;
        }
    }

    private static float GetAdvanceWidth(typography::Typography.OpenFont.Typeface typeface, int glyphIndex, int codepoint, float scale)
    {
        try
        {
            return typeface.GetHAdvanceWidthFromGlyphIndex(glyphIndex) * scale;
        }
        catch 
        {
            try
            {
                return typeface.GetAdvanceWidth(codepoint) * scale;
            }
            catch
            {
                return 0f;
            }
        }
    }
}
