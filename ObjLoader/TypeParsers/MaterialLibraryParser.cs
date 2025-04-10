using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class MaterialLibraryParser : TypeParserBase, IMaterialLibraryParser
    {
        private readonly MaterialLibraryLoaderFacade _libraryLoaderFacade;

        public MaterialLibraryParser(MaterialLibraryLoaderFacade libraryLoaderFacade)
        {
            _libraryLoaderFacade = libraryLoaderFacade;
        }

        protected override string Keyword => "mtllib";

        public override void Parse(string line) => _libraryLoaderFacade.Load(line);
    }
}