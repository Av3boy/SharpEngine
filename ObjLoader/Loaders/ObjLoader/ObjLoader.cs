using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.ObjLoader.Loader.Loaders;
using SharpEngine.Core.ObjLoader.TypeParsers;

using Silk.NET.OpenGL;

using System.Collections.Generic;
using System.IO.Abstractions;

namespace SharpEngine.Core.ObjLoader.Loaders.ObjLoader
{
    /// <summary>
    ///     Loads an OBJ model file and converts its parsed data into runtime Mesh instances.
    /// </summary>
    public class ObjLoader : LoaderBase
    {
        private readonly string _path;
        private readonly DataStore _dataStore;
        private readonly List<ITypeParser> _typeParsers = [];
        private readonly List<string> _unrecognizedLines = [];

        /// <summary>
        ///     Initializes a new instance of <see cref="ObjLoader"/> with the specified file path and data store.
        /// </summary>
        /// <param name="path">The path to the OBJ file.</param>
        /// <param name="dataStore">The data store to populate during parsing.</param>
        /// <param name="fileStreamFactory">Represents a factory for creating file streams.</param>
        /// <param name="fileManager">Represents a manager for handling file operations.</param>
        public ObjLoader(string path, DataStore dataStore, IFileStreamFactory fileStreamFactory, IFileManager fileManager) : base(fileStreamFactory, fileManager)
        {
            _path = path;
            _dataStore = dataStore;
        }

        /// <summary>
        ///     Adds the provided type parsers to the loader's internal list, enabling it to recognize and parse different line types in the OBJ file.
        /// </summary>
        /// <param name="parsers">The type parsers to add.</param>
        /// <returns>The current instance of <see cref="ObjLoader"/>.</returns>
        public ObjLoader SetupTypeParsers(params ITypeParser[] parsers)
        {
            foreach (var parser in parsers)
                _typeParsers.Add(parser);

            return this;
        }

        /// <inheritdoc />
        protected override void ParseLine(string keyword, string data)
        {
            foreach (var typeParser in _typeParsers)
                if (typeParser.CanParse(keyword))
                {
                    typeParser.Parse(data);
                    return;
                }

            _unrecognizedLines.Add(keyword + " " + data);
        }

        /// <summary>
        ///     Loads the OBJ file and produces a list of <see cref="Mesh"/> objects suitable for rendering.
        /// </summary>
        /// <param name="gl">The OpenGL context used to construct GPU resources.</param>
        public List<Mesh> Load(GL gl)
        {
            ParseFile(_path);

            return
            [
                new(gl)
                {
                    Vertices2 = _dataStore.Vertices,
                    TextureCoordinates2 = _dataStore.Textures,
                    Normals2 = _dataStore.Normals,
                    Groups = _dataStore.Groups,
                    Materials = _dataStore.Materials
                }
            ];
        }
    }
}