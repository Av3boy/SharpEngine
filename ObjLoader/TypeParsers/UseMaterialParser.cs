using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.ObjLoader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Parses material usage lines ("usemtl") and assigns materials to the current group.
    /// </summary>
    public class UseMaterialParser : TypeParserBase, ITypeParser
    {
        private readonly IMaterialDataStore _dataStore;

        /// <summary>
        ///     Initializes a new instance of <see cref="UseMaterialParser"/> with the specified data store.
        /// </summary>
        /// <param name="dataStore">The data store to use.</param>
        public UseMaterialParser(IMaterialDataStore dataStore)
        {
            _dataStore = dataStore;
        }
        
        /// <inheritdoc />
        protected override string Keyword => "usemtl";
        
        /// <inheritdoc />
        public override void Parse(string line) => _dataStore.SetMaterial(line);
    }
}