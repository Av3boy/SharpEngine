using SharpEngine.Core.Components.Properties.Meshes.MeshData;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    public interface IVertexDataStore
    {
        void AddVertex(Vertex vertex);
    }
}