using Microsoft.Extensions.Logging;
using SharpEngine.Telemetry;
using System.IO;

namespace ObjLoader.Loaders.MaterialLoader
{
    /// <summary>
    ///     A facade for the <see cref="MaterialLibraryLoader"/> to provide a simplified interface for loading material libraries.
    /// </summary>
    public class MaterialLibraryLoaderFacade : IMaterialLibraryLoaderFacade
    {
        private static readonly ILogger<MaterialLibraryLoaderFacade> Logger = LoggingExtensions.CreateLogger<MaterialLibraryLoaderFacade>();

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
                Logger.LogWarning("Material file '{MaterialFileName}' doesn't exist.", materialFileName);
                return;
            }    

            using var stream = _loader.Open(materialFileName);

            if (stream != null)
                _loader.Load(stream);
        }
    }
}