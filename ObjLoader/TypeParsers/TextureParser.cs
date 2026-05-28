using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Represents a parser for texture coordinate definitions in an OBJ file, responsible for parsing lines that define texture coordinates and storing them in the data store.
    /// </summary>
    public class TextureParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureParser"/>.
        /// </summary>
        /// <param name="dataStore">The data store to which the parsed texture coordinates will be added.</param>
        public TextureParser(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        protected override string Keyword => "vt";

        /// <inheritdoc />
        public override void Parse(string line)
        {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();

            var texture = new TextureCoordinate(x, y);
            _dataStore.Textures.Add(texture);
        }
    }
}