using ObjLoader.Loader.TypeParsers;
using ObjLoader.Loaders.MaterialLoader;

namespace ObjLoader.Loaders.ObjLoader
{
    public class ObjLoaderFactory
    {
        private readonly string _path;

        public ObjLoaderFactory(string path)
        {
            _path = path;
        }

        /// <inheritdoc />
        public ObjLoader Create()
        {
            var dataStore = new DataStore();
            
            var faceParser = new FaceParser(dataStore);
            var groupParser = new GroupParser(dataStore);
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);

            var materialLibraryLoader = new MaterialLibraryLoader(_path, dataStore);
            var materialLibraryParser = new MaterialLibraryParser(materialLibraryLoader);
            var useMaterialParser = new UseMaterialParser(dataStore);

            return new ObjLoader(_path, dataStore, faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);
        }
    }
}