using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class UseMaterialParser : TypeParserBase, IUseMaterialParser
    {
        private readonly IElementGroup _elementGroup;

        public UseMaterialParser(IElementGroup elementGroup)
        {
            _elementGroup = elementGroup;
        }
        
        /// <inheritdoc />
        protected override string Keyword => "usemtl";
        
        /// <inheritdoc />
        public override void Parse(string line) => _elementGroup.SetMaterial(line);
    }
}