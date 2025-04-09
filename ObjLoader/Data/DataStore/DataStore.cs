using ObjLoader.Loader.Common;
using ObjLoader.Loader.Data.Elements;

using SharpEngine.Core.Components.Obsolete.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData;

using System.Collections.Generic;
using System.Linq;

namespace ObjLoader.Loader.Data.DataStore
{
    public class DataStore : IDataStore, IGroupDataStore, IVertexDataStore, ITextureDataStore, INormalDataStore,
                             IFaceGroup, IMaterialLibrary, IElementGroup
    {
        private Group _currentGroup;

        private readonly List<Group> _groups = [];
        private readonly List<Material> _materials = [];

        private readonly List<Vertex> _vertices = [];
        private readonly List<Texture> _textures = [];
        private readonly List<Normal> _normals = [];

        public IList<Vertex> Vertices => _vertices;

        public IList<Texture> Textures => _textures;

        public IList<Normal> Normals => _normals;

        public IList<Material> Materials => _materials;

        public IList<Group> Groups => _groups;

        public void AddFace(Face face)
        {
            PushGroupIfNeeded();

            _currentGroup.AddFace(face);
        }

        public void PushGroup(string groupName)
        {
            _currentGroup = new Group(groupName);
            _groups.Add(_currentGroup);
        }

        private void PushGroupIfNeeded()
        {
            if (_currentGroup == null)
                PushGroup("default");
        }

        public void AddVertex(Vertex vertex) => _vertices.Add(vertex);

        public void AddTexture(Texture texture) => _textures.Add(texture);

        public void AddNormal(Normal normal) => _normals.Add(normal);

        public void Push(Material material) => _materials.Add(material);

        public void SetMaterial(string materialName)
        {
            var material = _materials.SingleOrDefault(x => x.Name.EqualsOrdinalIgnoreCase(materialName));
            PushGroupIfNeeded();
            _currentGroup.Material = material;
        }
    }
}