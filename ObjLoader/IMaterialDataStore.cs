namespace SharpEngine.Core.ObjLoader;

/// <summary>
///     Contains definitions for the material data store, which provides methods to manage material information during the OBJ file loading process.
/// </summary>
public interface IMaterialDataStore
{
    /// <summary>
    ///     Sets the material for the current group.
    /// </summary>
    /// <param name="materialName">The name of the material to set.</param>
    public void SetMaterial(string materialName);

}