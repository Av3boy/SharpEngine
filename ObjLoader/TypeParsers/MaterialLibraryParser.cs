using ObjLoader.Loader.Loaders;
using ObjLoader.TypeParsers;

namespace ObjLoader.Loader.TypeParsers
{
    public class MaterialLibraryParser : TypeParserBase, ITypeParser
    {
        private readonly IMaterialLibraryLoaderFacade _libraryLoaderFacade;

        public MaterialLibraryParser(MaterialLibraryLoader loader)
        {
            _libraryLoaderFacade = new MaterialLibraryLoaderFacade(loader);
        }

        /// <inheritdoc />
        protected override string Keyword => "mtllib";

        /// <inheritdoc />
        public override void Parse(string line) => _libraryLoaderFacade.Load(line);
    }
}