using ObjLoader.Loader.Common;
using ObjLoader.Loader.TypeParsers.Interfaces;
using SharpEngine.Core.Components.Obsolete.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData;

namespace ObjLoader.Loader.TypeParsers
{
    public class TextureParser : TypeParserBase, ITextureParser
    {
        private readonly ITextureDataStore _textureDataStore;

        public TextureParser(ITextureDataStore textureDataStore)
        {
            _textureDataStore = textureDataStore;
        }

        protected override string Keyword => "vt";

        public override void Parse(string line)
        {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();

            var texture = new TextureCoordinate(x, y);
            _textureDataStore.AddTexture(texture);
        }
    }
}