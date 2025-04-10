using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public class MaterialStreamProvider : IMaterialStreamProvider
    {
        private string _objPath;

        public MaterialStreamProvider(string objPath)
        {
            _objPath = objPath;
        }

        /// <inheritdoc />
        public Stream Open(string materialFilePath)
        {
            var dir = Path.GetDirectoryName(_objPath);
            var path = Path.Combine(dir, materialFilePath);
            return File.Open(path, FileMode.Open, FileAccess.Read);
        }
    }

    public class MaterialNullStreamProvider : IMaterialStreamProvider
    {
        public Stream Open(string materialFilePath)
        {
            return null;
        }
    }
}