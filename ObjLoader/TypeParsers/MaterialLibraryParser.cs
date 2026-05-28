using SharpEngine.Core.ObjLoader.Loaders.MaterialLoader;
using SharpEngine.Core.ObjLoader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Represents a parser for material library definitions in OBJ files, responsible for parsing lines that specify material libraries and delegating the loading of the material library to the provided loader facade.
    /// </summary>
    public class MaterialLibraryParser : TypeParserBase, ITypeParser
    {
        private readonly MaterialLibraryLoaderFacade _libraryLoaderFacade;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MaterialLibraryParser"/>.
        /// </summary>
        /// <param name="loader">The material library loader.</param>
        /// <param name="path">The path to the material library file.</param>
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