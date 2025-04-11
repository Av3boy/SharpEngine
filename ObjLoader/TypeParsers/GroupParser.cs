using ObjLoader.Data;
using ObjLoader.TypeParsers;

namespace ObjLoader.Loader.TypeParsers
{
    public class GroupParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

        public GroupParser(DataStore dataStore)
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