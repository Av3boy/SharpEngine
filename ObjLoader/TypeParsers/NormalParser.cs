using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Represents a parser for normal vector definitions in an OBJ file, responsible for parsing lines that define vertex normals and storing them in the data store.
    /// </summary>
    public class NormalParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NormalParser"/>.
        /// </summary>
        /// <param name="dataStore">The data store to which the parsed normals will be added.</param>
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