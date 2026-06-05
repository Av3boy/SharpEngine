using SharpEngine.Core.Components.Properties.Meshes.MeshData;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    /// <summary>
    ///     Contains definitions for the <see cref="IFaceGroup"/> interface, which represents a group of faces in an OBJ model, allowing for the organization and management of faces based on their associated materials or other grouping criteria.
    /// </summary>
    public interface IFaceGroup
    {
        /// <summary>
        ///     Adds a face to the face group. 
        /// </summary>
        /// <remarks>
        ///     This method allows for the inclusion of a face, represented by the <see cref="Face"/> class, into the group, enabling the organization of faces based on their associated materials or other grouping criteria.
        /// </remarks>
        /// <param name="face">The face to add to the group.</param>
        void AddFace(Face face);
    }
}