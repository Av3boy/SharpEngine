namespace Core;

public struct VertexData
{
    public const int Stride = 8 * sizeof(float);
    public const int VerticesSize = 3;
    public const int NormalsSize = 3;
    public const int TexCoordsSize = 2;

    public const int VerticesOffset = 0; // data starts with vertices
    public const int NormalsOffset = VerticesSize * sizeof(float);
    public const int TexCoordsOffset = (VerticesSize + NormalsSize) * sizeof(float);

    public VertexData() { }
}
