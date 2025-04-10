using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Textures;

public static class TextureExtensions
{

    // Multiple textures can be bound, if your shader needs more than just one.
    // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
    // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.

    /// <summary>
    ///     Activate the texture.
    /// </summary>
    /// <param name="unit">The bound texture location.</param>
    public static void Use(this Texture texture, TextureUnit unit)
    {
        Window.GL.ActiveTexture(unit);
        Window.GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
    }
}
