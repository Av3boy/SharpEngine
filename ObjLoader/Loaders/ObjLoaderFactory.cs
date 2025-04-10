using System.IO;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers;

namespace ObjLoader.Loader.Loaders
{
    public interface IMaterialStreamProvider
    {
        Stream Open(string materialFilePath);
    }

    public class ObjLoaderFactory : IObjLoaderFactory
    {
        private string _path;

        public ObjLoaderFactory(string path)
        {
            _path = path;
        }

        public IObjLoader Create()
            => Create(new MaterialStreamProvider(_path));

        public IObjLoader Create(IMaterialStreamProvider materialStreamProvider)
        {
            var dataStore = new DataStore();
            
            var faceParser = new FaceParser(dataStore);
            var groupParser = new GroupParser(dataStore);
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);

            var materialLibraryLoader = new MaterialLibraryLoader(dataStore);
            var materialLibraryLoaderFacade = new MaterialLibraryLoaderFacade(materialLibraryLoader, materialStreamProvider);
            var materialLibraryParser = new MaterialLibraryParser(materialLibraryLoaderFacade);
            var useMaterialParser = new UseMaterialParser(dataStore);

            return new ObjLoader(_path, dataStore, faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);
        }
    }
}