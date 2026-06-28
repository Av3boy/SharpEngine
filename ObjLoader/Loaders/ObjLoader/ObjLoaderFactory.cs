using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.ObjLoader.Loader.Loaders;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;
using SharpEngine.Core.ObjLoader.Loaders.MaterialLoader;

using Silk.NET.OpenGL;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace SharpEngine.Core.ObjLoader.Loaders.ObjLoader
{
    /// <summary>
    ///     Represents a testable wrapper for creating stream file related objects.
    /// </summary>
    public class FileManager : IFileManager
    {
        /// <inheritdoc />
        public StreamReader StreamReader(string path)
            => new(path);
    }

    /// <summary>
    ///     Handles loading 3D models.
    /// </summary>
    public static class ObjLoaderFactory
    {
        private static readonly FileSystem _fileSystem;
        private static readonly FileManager _fileManager;

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
        /// <returns>The model loaded from the file.</returns>
        /// <exception cref="NotSupportedException">Thrown when the file extension of the provided path is not recognized as a supported mesh format.</exception>
        public static Model Load(GL gl, string path)
            => Path.GetExtension(path) switch
            {
                FbxExtension => LoadFbx("", path),
                ObjExtension => LoadObj(gl, path),
                _ => throw new NotSupportedException($"The file extension {Path.GetExtension(path)} is not a supported mesh file.")
            };

        // TODO: #3 Load fbx mesh from file
        private static Model LoadFbx(string identifier, string meshFilePath)
        {
            throw new NotImplementedException();
        }

        private static Model LoadObj(GL gl, string path)
        {
            var meshes = LoadObjMeshes(gl, path);
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

            var materialLibraryLoader = new MaterialLibraryLoader(dataStore, _fileSystem.FileStream, _fileManager);

            var materialLoader = new MaterialLibraryLoaderFacade(materialLibraryLoader, path);
            var materialLibraryParser = new MaterialLibraryParser(materialLoader);
            var useMaterialParser = new UseMaterialParser(dataStore);

            var loader = new ObjLoader(path, dataStore, _fileSystem.FileStream, _fileManager)
                .SetupTypeParsers(faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);

            return loader.Load(gl);
        }
    }
}