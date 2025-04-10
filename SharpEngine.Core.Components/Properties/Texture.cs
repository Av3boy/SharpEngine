namespace SharpEngine.Core.Textures;

/// <summary>
///     Represents a texture program.
/// </summary>
public class Texture
{
    /// <summary>The OpenGL handle for the texture.</summary>
    public readonly uint Handle;

    /// <summary>
    ///     Initializes a new instance of <see cref="Texture"/>.
    /// </summary>
    /// <param name="glHandle">The handle to the texture.</param>
    public Texture(uint glHandle)
    {
        Handle = glHandle;
    }
}
