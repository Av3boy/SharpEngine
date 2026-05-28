using SharpEngine.Core.Components.Properties.Meshes.MeshData;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    /// <summary>
    ///     Defines storage operations for texture coordinate data parsed from an OBJ file.
    /// </summary>
    public interface ITextureDataStore
    {
        /// <summary>
        ///     Adds a texture coordinate to the underlying data store.
        /// </summary>
        /// <param name="texture">The texture coordinate to add.</param>
        void AddTexture(TextureCoordinate texture);
    }
}
