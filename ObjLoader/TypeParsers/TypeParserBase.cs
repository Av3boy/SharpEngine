using ObjLoader.Loader.Common;
using ObjLoader.TypeParsers;

namespace ObjLoader.Loader.TypeParsers
{
    public abstract class TypeParserBase : ITypeParser
    {
        protected abstract string Keyword { get; }

        public bool CanParse(string keyword) => keyword.EqualsOrdinalIgnoreCase(Keyword);

        public abstract void Parse(string line);
    }
}