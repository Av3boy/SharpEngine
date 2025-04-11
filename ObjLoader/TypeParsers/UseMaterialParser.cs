using ObjLoader.Data;
using ObjLoader.TypeParsers;

namespace ObjLoader.Loader.TypeParsers
{
    public class UseMaterialParser : TypeParserBase, ITypeParser
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