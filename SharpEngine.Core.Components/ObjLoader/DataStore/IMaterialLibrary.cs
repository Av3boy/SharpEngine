using SharpEngine.Core.Components.Properties;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    /// <summary>
    ///     Represents a storage for materials discovered while parsing an OBJ's associated MTL file.
    /// </summary>
    public interface IMaterialLibrary
    {
        /// <summary>
        ///     Adds a material to the library.
        /// </summary>
        /// <param name="material">The material to add.</param>
        void Push(Material material);
    }
}
