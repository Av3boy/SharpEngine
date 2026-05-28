namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    /// <summary>
    ///     Defines operations for managing groups of faces while parsing OBJ files.
    /// </summary>
    public interface IGroupDataStore : IDataStore
    {
        /// <summary>
        ///     Creates and activates a new face group with the provided name.
        /// </summary>
        /// <param name="groupName">The name of the group to push.</param>
        void PushGroup(string groupName);
    }
}