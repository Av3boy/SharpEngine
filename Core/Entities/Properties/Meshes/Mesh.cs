using SharpEngine.Core.Attributes;

namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Represents a game object mesh.
/// </summary>
public class Mesh
{
    /// <summary>Gets or sets a identifying name for the mesh.</summary>
    public string Name { get; set; } = "New Mesh";

    /// <summary>
    ///     Gets or sets the mesh vertices.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Vertices { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh normals.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] Normals { get; set; } = [];

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public float[] TextureCoordinates { get; set; } = [];

    /// <summary>
    ///     Gets or sets the indicies of the mesh.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public uint[] Indices { get; set; } = [];
}