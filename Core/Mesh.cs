namespace Core;

/// <summary>
///     Represents a game object mesh.
/// </summary>
public class Mesh
{
    /// <summary>
    ///     Gets or sets the mesh vertices.
    /// </summary>
    public float[] Vertices { get; set; }

    /// <summary>
    ///     Gets or sets the mesh normals.
    /// </summary>
    public float[] Normals { get; set; }

    /// <summary>
    ///     Gets or sets the mesh texture UV coordinates.
    /// </summary>
    public float[] TextureCoordinates { get; set; }
}