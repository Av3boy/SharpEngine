using SharpEngine.Core.ObjLoader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    public class UseMaterialParser : TypeParserBase, ITypeParser
    {
        private readonly DataStore _dataStore;

        public UseMaterialParser(DataStore dataStore)
        {
            _dataStore = dataStore;
        }
        
        /// <inheritdoc />
        protected override string Keyword => "usemtl";
        
        /// <inheritdoc />
        public override void Parse(string line) => _dataStore.SetMaterial(line);
    }
}