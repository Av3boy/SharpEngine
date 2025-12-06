using ObjLoader.Loader.TypeParsers;
using ObjLoader.Loaders.MaterialLoader;

using SharpEngine.Core.Entities.Properties.Meshes;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using Tutorial;

namespace ObjLoader.Loaders.ObjLoader
{
    /// <summary>
    ///     Handles loading 3D models.
    /// </summary>
    public static class ObjLoaderFactory
    {
        const string FbxExtension = ".fbx";
        const string ObjExtension = ".obj";

        /// <summary>
        ///     Loads a mesh based on the file extension of the provided path.
        /// </summary>
        /// <param name="gl">The OpenGL context where the model should be bound.</param>
        /// <param name="meshFilePath">Specifies the file path of the mesh to be loaded, which determines the loading method based on its extension.</param>
        /// <returns>The model loaded from the file.</returns>
        /// <exception cref="NotSupportedException">Thrown when the file extension of the provided path is not recognized as a supported mesh format.</exception>
        public static Model_Old Load(GL gl, string path)
            => Path.GetExtension(path) switch
            {
                FbxExtension => LoadFbx("", path),
                ObjExtension => LoadObj(gl, path),
                _ => throw new NotSupportedException($"The file extension {Path.GetExtension(path)} is not a supported mesh file.")
            };

        // TODO: #3 Load fbx mesh from file
        private static Model_Old LoadFbx(string identifier, string meshFilePath)
        {
            throw new NotImplementedException();
        }

        private static Model_Old LoadObj(GL gl, string path)
        {
            return new Model_Old(gl, path);

            // TOOD: Use the ObjLoader to load the model instead of the external library.

            /*var dataStore = new DataStore();

            var faceParser = new FaceParser(dataStore);
            var groupParser = new GroupParser(dataStore);
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);

            var materialLibraryLoader = new MaterialLibraryLoader(path, dataStore);
            var materialLibraryParser = new MaterialLibraryParser(materialLibraryLoader);
            var useMaterialParser = new UseMaterialParser(dataStore);

            var loader = new ObjLoader(path, dataStore).SetupTypeParsers(faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);
            return loader.Load(gl);
            */
        }
    }
}