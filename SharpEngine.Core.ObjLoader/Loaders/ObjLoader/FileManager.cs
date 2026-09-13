using SharpEngine.Core.ObjLoader.Loader.Loaders;
using System.IO;

namespace SharpEngine.Core.ObjLoader.Loaders.ObjLoader
{
    /// <summary>
    ///     Represents a testable wrapper for creating stream file related objects.
    /// </summary>
    public class FileManager : IFileManager
    {
        /// <inheritdoc />
        public StreamReader StreamReader(string path)
            => new(path);
    }
}