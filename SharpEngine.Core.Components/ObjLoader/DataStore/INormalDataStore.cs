using SharpEngine.Core.Components.Properties.Meshes.MeshData;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    /// <summary>
    ///     Defines storage operations for normal vector data parsed from an OBJ file.
    /// </summary>
    public interface INormalDataStore
    {
        /// <summary>
        ///     Adds a normal vector to the data store.
        /// </summary>
        /// <param name="normal">The normal vector to add.</param>
        void AddNormal(Normal normal);
    }
}
