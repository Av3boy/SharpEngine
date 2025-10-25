namespace ObjLoader.Loader.Data.Elements
{
    public class Face
    {
        public readonly List<FaceVertex> _vertices = [];

        public void AddVertex(FaceVertex vertex) => _vertices.Add(vertex);

        public FaceVertex this[int i] => _vertices[i];

        public int Count => _vertices.Count;
    }

    public struct FaceVertex
    {
        public FaceVertex(int vertexIndex, int textureIndex, int normalIndex) : this()
        {
            VertexIndex = vertexIndex;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
        }

        public int VertexIndex { get; set; }
        public int TextureIndex { get; set; }
        public int NormalIndex { get; set; }
    }
}