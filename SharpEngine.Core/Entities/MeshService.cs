using System.Collections.Generic;

namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Represents a service that loads meshes into the GPU.
/// </summary>
public class MeshService
{
    /// <summary>A global instance of the service.</summary>
    public static readonly MeshService Instance = new();

    private readonly Dictionary<string, Mesh> Meshes = [];

    private MeshService() { }

    /// <summary>
    ///     Loads a mesh into the mesh cache.
    /// </summary>
    /// <param name="identifier">Used to cache the loaded mesh.</param>
    /// <param name="mesh">The mesh that should be stored in to the GPU buffer.</param>
    /// <returns>The loaded mesh.</returns>
    public Mesh LoadMesh(string identifier, Mesh mesh)
    {
        if (Meshes.TryGetValue(identifier, out var cachedMesh))
            return cachedMesh;

        Meshes.Add(identifier, mesh);
        return mesh;
    }
}