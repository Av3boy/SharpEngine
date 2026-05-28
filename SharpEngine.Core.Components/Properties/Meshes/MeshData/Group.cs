using SharpEngine.Core.Components.ObjLoader.DataStore;

namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    /// <summary>
    ///     Represents a group of faces in a mesh, which can be used to organize faces that share the same material or other properties.
    /// </summary>
    public class Group : IFaceGroup
    {
        private readonly List<Face> _faces = [];

        /// <summary>
        ///     Initializes a new instance of the <see cref="Group"/>.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        public Group(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the group, which can be used to identify the group and associate it with specific materials or properties in the mesh.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>Gets or sets the material associated with the group, which defines the appearance of the faces in the group when rendered.</summary>
        public Material? Material { get; set; }

        /// <summary>Gets the list of faces that belong to the group.</summary>
        public IList<Face> Faces => _faces;

        /// <summary>
        ///     Adds a face to the group.
        /// </summary>
        /// <param name="face">The face to add to the group.</param>
        public void AddFace(Face face) => _faces.Add(face);
    }
}