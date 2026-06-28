using Microsoft.Extensions.Logging;
using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;
using SharpEngine.Core.ObjLoader.Loaders.MaterialLoader;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Telemetry;
using Silk.NET.OpenGL;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace SharpEngine.Core.ObjLoader.Loaders.ObjLoader
{
    /// <summary>
    ///     Handles loading 3D models.
    /// </summary>
    public static class ObjLoaderFactory
    {
        private static readonly FileSystem _fileSystem;
        private static readonly FileManager _fileManager;

        private readonly static ILogger _logger = LoggingExtensions.CreateLogger(typeof(ObjLoaderFactory));

        static ObjLoaderFactory()
        {
            _fileSystem = new FileSystem();
            _fileManager = new FileManager();
        }

        const string FbxExtension = ".fbx";
        const string ObjExtension = ".obj";

        /// <summary>
        ///     Loads a mesh based on the file extension of the provided path.
        /// </summary>
        /// <param name="gl">The OpenGL context where the model should be bound.</param>
        /// <param name="path">Specifies the file path of the mesh to be loaded, which determines the loading method based on its extension.</param>
        /// <param name="model">The model loaded from the file.</param>
        /// <returns>The model loaded from the file.</returns>
        /// <exception cref="NotSupportedException">Thrown when the file extension of the provided path is not recognized as a supported mesh format.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no meshes are loaded.</exception>
        public static bool Load(GL gl, string path, out Model? model)
        {
            try
            {
                string fileExtension = Path.GetExtension(path);
                model = fileExtension switch
                {
                    FbxExtension => LoadFbx("", path),
                    ObjExtension => LoadObj(gl, path),
                    _ => throw new NotSupportedException($"The file extension {fileExtension} is not a supported mesh file.")
                };

                _logger.LogDebug("Successfully loaded model '{name}' from path: {path}.", model.Name, path);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load model from path: {path}.", path);

                model = null;
                return false;
            }
        }

        // TODO: #3 Load fbx mesh from file
        private static Model LoadFbx(string identifier, string meshFilePath)
        {
            throw new NotImplementedException();
        }

        private static Model LoadObj(GL gl, string path)
        {
            var meshes = LoadObjMeshes(gl, path);

            if (meshes.Count == 0)
                throw new InvalidOperationException("The .obj file contains no meshes.");

            return new Model(gl, path, meshes);
        }

        private static List<Mesh> LoadObjMeshes(GL gl, string path)
        {
            var dataStore = new DataStore();

            var faceParser = new FaceParser(dataStore);
            var groupParser = new GroupParser(dataStore);
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);
            var smoothingGroupParser = new SmoothingGroupParser(dataStore);
            var objectNameParser = new ObjectNameParser(dataStore);

            var materialLibraryLoader = new MaterialLibraryLoader(dataStore, _fileSystem.FileStream, _fileManager);

            var materialLoader = new MaterialLibraryLoaderFacade(materialLibraryLoader, path);
            var materialLibraryParser = new MaterialLibraryParser(materialLoader);
            var useMaterialParser = new UseMaterialParser(dataStore);

            var loader = new ObjLoader(path, dataStore, _fileSystem.FileStream, _fileManager)
                .SetupTypeParsers(faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser, smoothingGroupParser, objectNameParser);

            return loader.Load(gl);
        }
    }
}