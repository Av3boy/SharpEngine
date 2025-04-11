using System.Collections.Generic;
using ObjLoader.Loader.Data.Elements;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData;

namespace ObjLoader.Loader.Loaders
{
    public class LoadResult  
    {
        public IList<Vertex> Vertices { get; set; }
        public IList<TextureCoordinate> Textures { get; set; }
        public IList<Normal> Normals { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Material> Materials { get; set; }
    }
}