using SharpEngine.Core.Components.Properties.Meshes.MeshData;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    /// <summary>
    ///     Defines storage operations for vertex data parsed from an OBJ file.
    /// </summary>
    public interface IVertexDataStore
    {
        /// <summary>
        ///     Adds a vertex to the underlying data store.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        void AddVertex(Vertex vertex);
    }
}
