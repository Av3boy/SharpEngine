namespace SharpEngine.Core.ObjLoader;

/// <summary>
///     Holds the name of the object being parsed, allowing it to be set during parsing and retrieved later.
/// </summary>
public interface IObjectNameDataStore
{
    /// <summary>
    ///     Sets the name of the object being parsed.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    void SetObjectName(string name);
}