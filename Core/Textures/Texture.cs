using Silk.NET.OpenGL;

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

    // Multiple textures can be bound, if your shader needs more than just one.
    // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
    // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.

    /// <summary>
    ///     Activate the texture.
    /// </summary>
    /// <param name="unit">The bound texture location.</param>
    public void Use(TextureUnit unit)
    {
        Window.GL.ActiveTexture(unit);
        Window.GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
}
