namespace SharpEngine.Core.ObjLoader.Loaders.MaterialLoader
{
    /// <summary>
    ///     Provides a facade for loading material library files into the application's material system.
    /// </summary>
    public interface IMaterialLibraryLoaderFacade
    {
        /// <summary>
        ///     Loads the specified material file into the application's material system.
        /// </summary>
        /// <param name="materialFileName">The name of the material file to load.</param>
        void Load(string materialFileName);
    }
}