using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Shared.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace SharpEngine.Core.ObjLoader
{
    /// <summary>
    ///     Stores parsed OBJ file data such as vertices, normals, texture coordinates, materials and groups.
    ///     This type is used by the OBJ loader type parsers to collect geometry and material information.
    /// </summary>
    /// <summary>
    ///     Stores parsed OBJ file data such as vertices, normals, texture coordinates, materials and groups.
    ///     This type is used by the OBJ loader type parsers to collect geometry and material information.
    /// </summary>
    public class DataStore : IGroupDataStore, IFaceGroup, ITextureDataStore, INormalDataStore, IVertexDataStore, IMaterialDataStore
    {
        private Group _currentGroup = null!;

        /// <summary>Gets the list of parsed vertices.</summary>
        public List<Vertex> Vertices { get; } = [];

        /// <summary>Gets the list of parsed texture coordinates.</summary>
        public List<TextureCoordinate> Textures { get; } = [];

        /// <summary>Gets the list of parsed normals.</summary>
        public List<Normal> Normals { get; } = [];

        /// <summary>Gets the collection of materials discovered in the file.</summary>
        public List<Material> Materials { get; } = [];

        /// <summary>Gets the list of groups present in the file.</summary>
        public List<Group> Groups { get; } = [];

        /// <summary>
        ///     Initializes a new instance of <see cref="DataStore"/> and creates a default group.
        /// </summary>
        public DataStore()
        {
            PushGroup("default");
        }

        /// <summary>
        ///     Adds a parsed face to the active group.
        /// </summary>
        /// <param name="face">The parsed face to add.</param>
        public void AddFace(Face face)
            => _currentGroup.AddFace(face);

        /// <summary>
        ///     Creates and activates a new group with the given name.
        /// </summary>
        /// <param name="groupName">The name of the group to create.</param>
        public void PushGroup(string groupName)
        {
            _currentGroup = new Group(groupName);
            Groups.Add(_currentGroup);
        }

        /// <inheritdoc />
        public void SetMaterial(string materialName)
        {
            // TOOD: This should probably throw or handle the case where the material is not found, but for now we'll just set it to null.
            var material = Materials.SingleOrDefault(x => x.Name.EqualsOrdinalIgnoreCase(materialName));
            _currentGroup.Material = material;
        }

        /// <inheritdoc />
        public void AddTexture(TextureCoordinate texture)
            => Textures.Add(texture);
        
        /// <inheritdoc />
        public void AddNormal(Normal normal)
            => Normals.Add(normal);
        
        /// <inheritdoc />
        public void AddVertex(Vertex vertex)
            => Vertices.Add(vertex);

        /// <inheritdoc />
        public void AddMaterial(Material currentMaterial)
            => Materials.Add(currentMaterial);
    }
}