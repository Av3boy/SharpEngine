using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.ObjLoader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Parses group definition lines ("g") and pushes a new group into the data store.
    /// </summary>
    public class GroupParser : TypeParserBase, ITypeParser
    {
        private readonly IGroupDataStore _dataStore;

        /// <summary>
        ///     Initializes a new instance of <see cref="GroupParser"/> with the specified data store to populate during parsing.
        /// </summary>
        /// <param name="dataStore">The data store to populate.</param>
        public GroupParser(IGroupDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        protected override string Keyword => "g";

        /// <inheritdoc />
        public override void Parse(string line)
            => _dataStore.PushGroup(line);
    }
}