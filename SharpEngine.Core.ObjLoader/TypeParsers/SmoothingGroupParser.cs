using SharpEngine.Core.ObjLoader.Loader.TypeParsers;
using System;

namespace SharpEngine.Core.ObjLoader.TypeParsers;

/// <summary>
///     Parses smoothing group definitions ("s") and populates the current group's smoothing group information.
/// </summary>
public class SmoothingGroupParser : TypeParserBase, ITypeParser
{
    private ISmoothingGroupDataStore _dataStore;
    
    /// <summary>
    ///   Initializes a new instance of <see cref="SmoothingGroupParser"/>.
    /// </summary>
    /// <param name="dataStore">The data store to store the parsed smoothing group information.</param>
    public SmoothingGroupParser(ISmoothingGroupDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    /// <inheritdoc />
    protected override string Keyword => "s";

    /// <inheritdoc />
    /// <remarks>
    ///     The line is expected to be in the format "group_number" or "off" so the s at the beginning is not included in the data string passed to this method.
    /// </remarks>
    /// <exception cref="FormatException">Thrown when the line format is invalid or the smoothing group number is not a valid integer.</exception>
    public override bool Parse(string data)
    {
        if (data.Equals("off", StringComparison.OrdinalIgnoreCase))
        {
            _dataStore.SetSmoothingGroupOff();
            return true;
        }

        // Parse the smoothing group number
        if (!int.TryParse(data, out int groupNumber))
            throw new FormatException($"Invalid smoothing group identifier: {data}");

        _dataStore.SetSmoothingGroup(groupNumber);
        return true;
    }
}
