namespace SharpEngine.Core.Shaders;

/// <summary>Contains the attribute names for the shaders.</summary>
public static class ShaderAttributes
{
    /// <summary>Represents the position attribute.</summary>
    public const string Pos = "aPos";

    /// <summary>Represents the normal attribute.</summary>
    public const string Normal = "aNormal";

    /// <summary>Represents the texture coordinates attribute.</summary>
    public const string TexCoords = "aTexCoords";

    /// <summary>Represents value given when a attribute location is not found.</summary>
    public const int AttributeLocationNotFound = -1;

    /// <summary>Represents the model matrix attribute.</summary>
    public const string Model = "model";
}
