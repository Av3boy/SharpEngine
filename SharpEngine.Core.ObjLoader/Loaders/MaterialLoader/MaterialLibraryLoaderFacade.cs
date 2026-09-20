using Microsoft.Extensions.Logging;
using SharpEngine.Telemetry;
using System.IO;

namespace SharpEngine.Core.ObjLoader.Loaders.MaterialLoader
{
    /// <summary>
    ///     A facade for the <see cref="MaterialLibraryLoader"/> to provide a simplified interface for loading material libraries.
    /// </summary>
    public class MaterialLibraryLoaderFacade : IMaterialLibraryLoaderFacade
    {
        private readonly ILogger<MaterialLibraryLoaderFacade> _logger;
        private readonly MaterialLibraryLoader _loader;
        private readonly string _path;

        /// <summary>
        ///     Initializes a new instance of <see cref="MaterialLibraryLoaderFacade"/>.
        /// </summary>
        /// <param name="loader">Provides the functionality to load material libraries.</param>
        /// <param name="path">Optional path where the material file should exist.</param>
        /// <param name="logger">The logger to use for logging messages.</param>
        public MaterialLibraryLoaderFacade(MaterialLibraryLoader loader, string path, ILogger<MaterialLibraryLoaderFacade>? logger = null)
        {
            _loader = loader;
            _path = path;

            _logger = logger ?? LoggingExtensions.CreateLogger<MaterialLibraryLoaderFacade>();
        }

        /// <inheritdoc />
        public void Load(string materialFileName)
        {
            string materialFilePath = Path.Combine(Path.GetDirectoryName(_path)!, materialFileName);
            if (!File.Exists(materialFilePath))
            {
                _logger.LogWarning("Material file '{MaterialFileName}' doesn't exist.", materialFileName);
                return;
            }

            _loader.ParseFile(materialFilePath);
        }
    }
}