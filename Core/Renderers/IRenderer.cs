namespace Core.Renderers;

/// <summary>
///     Contains definitions shared across all renderers.
/// </summary>
public abstract class RendererBase
{
    /// <summary>
    ///     Initializes the renderer.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    ///    Renders the scene.
    /// </summary>
    public abstract void Render();
}
