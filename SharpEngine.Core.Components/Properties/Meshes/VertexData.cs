namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Represents the data of a vertex.
/// </summary>
public class VertexData
{
    public const int VertexIndex = 0;
    public const int NormalIndex = 1;
    public const int TexCoordIndex = 2;

    /// <summary>The stride or length of the vertex data object.</summary>
    public const int Stride = VertexCount + NormalCount + TexCoordCount;

    /// <summary>The size of the vertices in the vertex data object.</summary>
    public const int VertexCount = 3;

    /// <summary>The size of the normals in the vertex data object.</summary>
    public const int NormalCount = 3;

    /// <summary>The size of the UV texture coordinates in the vertex data object.</summary>
    public const int TexCoordCount = 2;

    /// <summary>The byte offset to the vertices in the vertex data object.</summary>
    public const int VerticesOffset = 0; // data starts with vertices

    /// <summary>The byte offset to the normals in the vertex data object.</summary>
    public const int NormalsOffset = VerticesOffset + VertexCount;

    /// <summary>The byte offset to the texture coordinates in the vertex data object.</summary>
    public const int TexCoordsOffset = NormalsOffset + NormalCount;
}
