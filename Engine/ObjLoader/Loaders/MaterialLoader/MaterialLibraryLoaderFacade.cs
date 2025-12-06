using SharpEngine.Shared;
using System.IO;

namespace ObjLoader.Loaders.MaterialLoader
{
    public class MaterialLibraryLoaderFacade : IMaterialLibraryLoaderFacade
    {
        private readonly MaterialLibraryLoader _loader;

        /// <summary>
        ///     Initializes a new instance of <see cref="MaterialLibraryLoaderFacade"/>.
        /// </summary>
        /// <param name="loader">Provides the functionality to load material libraries.</param>
        public MaterialLibraryLoaderFacade(MaterialLibraryLoader loader)
        {
            _loader = loader;
        }

        /// <inheritdoc />
        public void Load(string materialFileName)
        {
            if (!File.Exists(materialFileName))
            {
                Debug.Log.Warning("Material file '{MaterialFileName}' doesn't exist.", materialFileName);
                return;
            }    

            using var stream = _loader.Open(materialFileName);

            if (stream != null)
                _loader.Load(stream);
        }
    }
}