using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

using System;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Parses vertex position lines ("v x y z") and adds them to the data store.
    /// </summary>
    public class VertexParser : TypeParserBase, ITypeParser
    {
        private readonly IVertexDataStore _dataStore;

        /// <summary>
        ///     Initializes a new instance of <see cref="VertexParser"/> with the specified data store to populate.
        /// </summary>
        /// <param name="dataStore">The data store to populate.</param>
        public VertexParser(IVertexDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        protected override string Keyword => "v";

        /// <inheritdoc />
        public override bool Parse(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            var vertex = new Vertex()
            {
                Position = new System.Numerics.Vector3(x, y, z),
            };

            _dataStore.AddVertex(vertex);

            return true;
        }
    }
}