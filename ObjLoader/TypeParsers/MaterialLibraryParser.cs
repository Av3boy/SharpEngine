using SharpEngine.Core.ObjLoader.Loaders.MaterialLoader;
using SharpEngine.Core.ObjLoader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
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