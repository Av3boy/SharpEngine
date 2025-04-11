using ObjLoader.Loader.Common;
using ObjLoader.TypeParsers;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using System;

namespace ObjLoader.Loader.TypeParsers
{
    public class VertexParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

        public VertexParser(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        protected override string Keyword => "v";

        /// <inheritdoc />
        public override void Parse(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            var vertex = new Vertex(x, y, z);
            _dataStore.Vertices.Add(vertex);
        }
    }
}