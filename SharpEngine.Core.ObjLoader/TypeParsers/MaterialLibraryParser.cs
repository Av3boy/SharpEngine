using SharpEngine.Core.ObjLoader.Loaders.MaterialLoader;
using SharpEngine.Core.ObjLoader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Represents a parser for material library definitions in OBJ files, responsible for parsing lines that specify material libraries and delegating the loading of the material library to the provided loader facade.
    /// </summary>
    public class MaterialLibraryParser : TypeParserBase, ITypeParser
    {
        private readonly IMaterialLibraryLoaderFacade _libraryLoaderFacade;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MaterialLibraryParser"/>.
        /// </summary>
        /// <param name="facade">The material library loader facade.</param>
        public MaterialLibraryParser(IMaterialLibraryLoaderFacade facade) 
        {
            _libraryLoaderFacade = facade;
        }

        /// <inheritdoc />
        protected override string Keyword => "mtllib";

        /// <inheritdoc />
        public override bool Parse(string line)
        {
            _libraryLoaderFacade.Load(line);
            return true;
        }
    }
}