namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    /// <summary>
    ///     Represents a face in a 3D mesh, which is defined by a list of vertices.
    /// </summary>
    /// <remarks>
    ///     Each vertex is represented by a <see cref="FaceVertex"/> struct that contains indices for the vertex position, texture coordinate, and normal vector.
    /// </remarks>
    public class Face
    {
        private readonly List<FaceVertex> _vertices = [];

        /// <summary>
        ///     Adds a vertex to the face.
        /// </summary>  
        /// <param name="vertex">The vertex to add.</param>
        public void AddVertex(FaceVertex vertex) => _vertices.Add(vertex);

        /// <summary>
        ///     An indexer to access the vertices of the face by their index.
        /// </summary>
        /// <param name="i">The index of the vertex to access.</param>
        /// <returns>The vertex at the specified index.</returns>
        public FaceVertex this[int i] => _vertices[i];

        /// <summary>Gets the number of vertices in the face.</summary>
        public int Count => _vertices.Count;
    }

    /// <summary>
    ///     Represents a vertex in a face, which contains indices for the vertex position, texture coordinate, and normal vector.
    /// </summary>
    public struct FaceVertex
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FaceVertex"/>.
        /// </summary>
        /// <param name="vertexIndex">The index of the vertex position.</param>
        /// <param name="textureIndex">The index of the texture coordinate.</param>
        /// <param name="normalIndex">The index of the normal vector.</param>
        public FaceVertex(int vertexIndex, int textureIndex, int normalIndex)
        {
            VertexIndex = vertexIndex;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
        }

        /// <summary>
        ///     Gets or sets the index of the vertex position in the mesh's vertex list.
        /// </summary>
        public int VertexIndex { get; set; }

        /// <summary>Gets or sets the index of the texture coordinate in the mesh's texture coordinate list.</summary>
        public int TextureIndex { get; set; }

        /// <summary>Gets or sets the index of the normal vector in the mesh's normal vector list.</summary>
        public int NormalIndex { get; set; }
    }
}