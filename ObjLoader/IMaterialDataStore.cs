using SharpEngine.Core.Components.Properties;

namespace SharpEngine.Core.ObjLoader;

/// <summary>
///     Contains definitions for the material data store, which provides methods to manage material information during the OBJ file loading process.
/// </summary>
public interface IMaterialDataStore
{
    /// <summary>
    ///     Adds a material to the material data store.
    /// </summary>
    /// <param name="currentMaterial">The material to add.</param>
    void AddMaterial(Material currentMaterial);

    /// <summary>
    ///     Sets the material for the current group.
    /// </summary>
    /// <param name="materialName">The name of the material to set.</param>
    void SetMaterial(string materialName);
}