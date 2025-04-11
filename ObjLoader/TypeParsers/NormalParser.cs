using ObjLoader.Loader.Common;
using ObjLoader.TypeParsers;
using SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData;

namespace ObjLoader.Loader.TypeParsers
{
    public class NormalParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

        public NormalParser(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        protected override string Keyword => "vn";

        /// <inheritdoc />
        public override void Parse(string line)
        {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            var normal = new Normal(x, y, z);
            _dataStore.Normals.Add(normal);
        }
    }
}