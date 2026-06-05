using System.Collections.Generic;

namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Service responsible for caching and providing meshes bound to GPU resources.
/// </summary>
public class MeshService : IMeshService
{
    /// <summary>A global instance of the service.</summary>
    public static readonly MeshService Instance = new();

    private readonly Dictionary<string, Mesh> Meshes = [];

    private MeshService() { }

    /// <inheritdoc />
    public Mesh LoadMesh(string identifier, Mesh mesh)
    {
        if (Meshes.TryGetValue(identifier, out var cachedMesh))
            return cachedMesh;

        Meshes.Add(identifier, mesh);
        return mesh;
    }
}

/// <summary>
///     Provides methods to load and retrieve Mesh instances from a cache keyed by an identifier.
/// </summary>
public interface IMeshService
{
    /// <summary>
    ///     Loads a mesh into the mesh cache or returns an existing cached instance for the identifier.
    /// </summary>
    /// <param name="identifier">The cache key used to identify the mesh.</param>
    /// <param name="mesh">The mesh to cache if not already present.</param>
    /// <returns>The cached or newly stored <see cref="Mesh"/> instance.</returns>
    Mesh LoadMesh(string identifier, Mesh mesh);
}