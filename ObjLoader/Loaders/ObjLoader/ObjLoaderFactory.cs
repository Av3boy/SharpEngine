using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.TypeParsers;
using ObjLoader.Loaders.MaterialLoader;

namespace ObjLoader.Loaders.ObjLoader
{
    public static class ObjLoaderFactory
    {
        /// <inheritdoc />
        public static LoadResult Load(string path)
        {
            var dataStore = new DataStore();
            
            var faceParser = new FaceParser(dataStore);
            var groupParser = new GroupParser(dataStore);
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);

            var materialLibraryLoader = new MaterialLibraryLoader(path, dataStore);
            var materialLibraryParser = new MaterialLibraryParser(materialLibraryLoader);
            var useMaterialParser = new UseMaterialParser(dataStore);

            var loader = new ObjLoader(path, dataStore).SetupTypeParsers(faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);
            return loader.Load();
        }
    }
}