using ObjLoader.Loader.Loaders;
using ObjLoader.Loaders.MaterialLoader;
using ObjLoader.TypeParsers;

namespace ObjLoader.Loader.TypeParsers
{
    public class MaterialLibraryParser : TypeParserBase, ITypeParser
    {
        private readonly IMaterialLibraryLoaderFacade _libraryLoaderFacade;

        public MaterialLibraryParser(MaterialLibraryLoader loader, string path)
        {
            _libraryLoaderFacade = new MaterialLibraryLoaderFacade(loader, path);
        }

        /// <inheritdoc />
        protected override string Keyword => "mtllib";

        /// <inheritdoc />
        public override void Parse(string line) => _libraryLoaderFacade.Load(line);
    }
}