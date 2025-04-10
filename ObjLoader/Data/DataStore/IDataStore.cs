using System.Collections.Generic;
using ObjLoader.Loader.Data.Elements;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData;

namespace ObjLoader.Loader.Data.DataStore
{
    public interface IDataStore 
    {
        IList<Vertex> Vertices { get; }
        IList<TextureCoordinate> Textures { get; }
        IList<Normal> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}