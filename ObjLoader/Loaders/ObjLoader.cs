using ObjLoader.Data;
using ObjLoader.TypeParsers;

using System.Collections.Generic;
using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public class ObjLoader : LoaderBase, IObjLoader
    {
        private string _path;
        private readonly DataStore _dataStore;
        private readonly List<ITypeParser> _typeParsers = [];
        private readonly List<string> _unrecognizedLines = [];

        public ObjLoader(
            string path,
            DataStore dataStore,
            ITypeParser faceParser,
            ITypeParser groupParser,
            ITypeParser normalParser,
            ITypeParser textureParser,
            ITypeParser vertexParser,
            ITypeParser materialLibraryParser,
            ITypeParser useMaterialParser)
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