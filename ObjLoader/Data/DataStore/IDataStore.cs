using System.Collections.Generic;
using ObjLoader.Loader.Data.Elements;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData;

namespace ObjLoader.Loader.Data.DataStore
{
    public interface IDataStore 
    {
        List<Vertex> Vertices { get; }
        List<TextureCoordinate> Textures { get; }
        List<Normal> Normals { get; }
        List<Material> Materials { get; }
        List<Group> Groups { get; }
    }
}