using System.IO;

namespace SharpEngine.Core.ObjLoader.Loader.Loaders
{
    /// <summary>
    ///     Represents an abstraction of a stream reader for testability.
    /// </summary>
    public interface IFileManager
    {
        /// <inheritdoc cref="StreamReader.StreamReader(string)"/>
        StreamReader StreamReader(string path);
    }
}