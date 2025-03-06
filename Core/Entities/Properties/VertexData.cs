namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents the data of a vertex.
/// </summary>
public struct VertexData
{
    /// <summary>The stride or lenght of the vertex data object.</summary>
    public const int Stride = 8 * sizeof(float);

    /// <summary>The size of the vertices in the vertex data object.</summary>
    public const int VerticesSize = 3;

    /// <summary>The size of the normals in the vertex data object.</summary>
    public const int NormalsSize = 3;

    /// <summary>The size of the UV texture coordinates in the vertex data object.</summary>
    public const int TexCoordsSize = 2;

    /// <summary>The byte offset to the vertices in the vertex data object.</summary>
    public const int VerticesOffset = 0; // data starts with vertices

    /// <summary>The byte offset to the normals in the vertex data object.</summary>
    public const int NormalsOffset = VerticesSize * sizeof(float);

    /// <summary>The byte offset to the texture coordinates in the vertex data object.</summary>
    public const int TexCoordsOffset = (VerticesSize + NormalsSize) * sizeof(float);

    /// <summary>
    ///     Initializes a new instance of <see cref="VertexData"/>.
    /// </summary>
    public VertexData() { }
}
