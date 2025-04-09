using SharpEngine.Core.Entities.Properties.Meshes;

namespace SharpEngine.Core.Interfaces;

// TODO: Make an abstract class and move Render method here?

/// <summary>
///     Contains definitions for rendering objects.
/// </summary>
public interface IRenderable
{
    /// <summary>
    ///     Gets or sets the vertex array object of the object.
    /// </summary>
    public uint VAO { get; set; }

    /// <summary>
    ///     Initializes the necessary buffers for the object.
    /// </summary>
    /// <param name="mesh">The mesh that should the buffers should be initialized with.</param>
    public void InitializeBuffers(Mesh mesh);

    /// <summary>
    ///     Binds the VAO to the current context.
    /// </summary>
    public void Bind();

}
