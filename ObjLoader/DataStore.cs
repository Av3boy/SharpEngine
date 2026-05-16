using ObjLoader.Loader.Common;
using ObjLoader.Loader.Data.Elements;

using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using System.Collections.Generic;
using System.Linq;

namespace ObjLoader
{
    public class DataStore : IGroupDataStore, IFaceGroup
    {
        private Group _currentGroup;

        public List<Vertex> Vertices { get; } = [];

        public List<TextureCoordinate> Textures { get; } = [];

        public List<Normal> Normals { get; } = [];

        public List<Material> Materials { get; } = [];

        public List<Group> Groups { get; } = [];

        public DataStore()
        {
            PushGroup("default");
        }

        public void AddFace(Face face)
        {
            _currentGroup.AddFace(face);
        }

        public void PushGroup(string groupName)
        {
            _currentGroup = new Group(groupName);
        Groups.Add(_currentGroup);
        }

        /// <inheritdoc />
        public void SetMaterial(string materialName)
        {
        var material = Materials.SingleOrDefault(x => x.Name.EqualsOrdinalIgnoreCase(materialName));
            _currentGroup.Material = material;
        }
    }
}