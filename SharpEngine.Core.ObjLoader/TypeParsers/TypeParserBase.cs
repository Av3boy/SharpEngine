using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Base implementation for OBJ file line type parsers. Provides a simple keyword matching helper.
    /// </summary>
    public abstract class TypeParserBase : ITypeParser
    {
        /// <summary>
        ///     The keyword this parser recognizes (e.g. "v", "vn", "f").
        /// </summary>
        protected abstract string Keyword { get; }

        /// <inheritdoc />
        public bool CanParse(string keyword) => keyword.EqualsOrdinalIgnoreCase(Keyword);

        /// <inheritdoc />
        public abstract bool Parse(string line);
    }
}