namespace SharpEngine.Core.ObjLoader;

/// <summary>
///     Represents a data store for smoothing group information in an OBJ file, allowing the storage of smoothing group lines for later processing.
/// </summary>
public interface ISmoothingGroupDataStore
{
    /// <summary>
    ///     Sets the current smoothing group to the specified group number.
    /// </summary>
    /// <param name="groupNumber">The smoothing group number.</param>
    void SetSmoothingGroup(int groupNumber);

    /// <summary>
    ///     Turns off the current smoothing group.
    /// </summary>
    void SetSmoothingGroupOff();
}