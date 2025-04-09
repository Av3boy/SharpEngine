using ObjLoader.Loader.TypeParsers.Interfaces;
using SharpEngine.Core.Components.Obsolete.ObjLoader.DataStore;

namespace ObjLoader.Loader.TypeParsers
{
    public class GroupParser : TypeParserBase, IGroupParser
    {
        private readonly IGroupDataStore _groupDataStore;

        public GroupParser(IGroupDataStore groupDataStore)
        {
            _groupDataStore = groupDataStore;
        }

        protected override string Keyword => "g";

        public override void Parse(string line) => _groupDataStore.PushGroup(line);
    }
}