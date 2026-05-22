namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Contains the different types of data the .obj file contains.
/// </summary>
public static class ObjMeshData
{
    /// <summary>Represents a line of vertex data.</summary>
    public const string VERTEX = "v";

    /// <summary>Represents a line of vertex normal data.</summary>
    public const string VERTEX_NORMAL = "vn";

    /// <summary>Represents a line of uv coordinate data.</summary>
    public const string VERTEX_TEXTURE = "vt";

    /// <summary>Represents a line of vertex indicie data.</summary>
    public const string FACE = "f";

    /// <summary>Represents that a new object begins.</summary>
    public const string OBJECT = "o";
}