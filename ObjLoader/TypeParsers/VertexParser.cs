using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

using System;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
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

            var vertex = new Vertex()
            {
                Position = new System.Numerics.Vector3(x, y, z),
            };
            _dataStore.Vertices.Add(vertex);
        }
    }
}