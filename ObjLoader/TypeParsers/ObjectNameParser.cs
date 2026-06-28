using SharpEngine.Core.ObjLoader.Loader.TypeParsers;
using System;

namespace SharpEngine.Core.ObjLoader.TypeParsers;

/// <summary>
///    Parses object name lines `o object_name` and handles the object name accordingly.
/// </summary>
public class ObjectNameParser : TypeParserBase, ITypeParser
{
    private IObjectNameDataStore _dataStore;

    /// <summary>
    ///   Initializes a new instance of <see cref="ObjectNameParser"/>.
    /// </summary>
    /// <param name="dataStore">The data store to store the parsed object name.</param>
    public ObjectNameParser(IObjectNameDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    /// <inheritdoc />
    protected override string Keyword => "o";

    /// <inheritdoc />
    /// <exception cref="FormatException">Thrown when the line format is invalid.</exception>
    public override bool Parse(string data)
    {
        _dataStore.SetObjectName(data);

        return true;
    }
}
