using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    public abstract class TypeParserBase : ITypeParser
    {
        protected abstract string Keyword { get; }

        public bool CanParse(string keyword) => keyword.EqualsOrdinalIgnoreCase(Keyword);

        public abstract void Parse(string line);
    }
}