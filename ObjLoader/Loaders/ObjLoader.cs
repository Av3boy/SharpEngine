using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public class ObjLoader : LoaderBase, IObjLoader
    {
        private string _path;
        private readonly IDataStore _dataStore;
        private readonly List<ITypeParser> _typeParsers = [];
        private readonly List<string> _unrecognizedLines = [];

        public ObjLoader(
            string path,
            IDataStore dataStore,
            IFaceParser faceParser,
            IGroupParser groupParser,
            INormalParser normalParser,
            ITextureParser textureParser,
            IVertexParser vertexParser,
            IMaterialLibraryParser materialLibraryParser,
            IUseMaterialParser useMaterialParser)
        {
            _path = path;

            _dataStore = dataStore;
            SetupTypeParsers(
                vertexParser,
                faceParser,
                normalParser,
                textureParser,
                groupParser,
                materialLibraryParser,
                useMaterialParser);
        }

        private void SetupTypeParsers(params ITypeParser[] parsers)
        {
            foreach (var parser in parsers)
            {
                _typeParsers.Add(parser);
            }
        }

        protected override void ParseLine(string keyword, string data)
        {
            foreach (var typeParser in _typeParsers)
            {
                if (typeParser.CanParse(keyword))
                {
                    typeParser.Parse(data);
                    return;
                }
            }

            _unrecognizedLines.Add(keyword + " " + data);
        }

        public LoadResult Load()
        {
            var fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
            StartLoad(fileStream);

            return new()
            {
                Vertices = _dataStore.Vertices,
                Textures = _dataStore.Textures,
                Normals = _dataStore.Normals,
                Groups = _dataStore.Groups,
                Materials = _dataStore.Materials
            };
        }
    }
}