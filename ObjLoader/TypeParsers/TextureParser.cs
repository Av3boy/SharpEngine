using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    public class TextureParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

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