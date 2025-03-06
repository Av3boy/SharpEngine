using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Shaders;
using System.Numerics;

namespace SharpEngine.Core.Entities.Views;

/// <summary>
///    Represents a view in the editor.
/// </summary>
public class View
{
    /// <summary>
    ///     Initializes a new instance of <see cref="View"/>.
    /// </summary>
    /// <param name="settings">The settings for the view.</param>
    public View(IViewSettings settings)
    {
        Settings = settings;
    }

    /// <summary>Gets or sets the settings for the view.</summary>
    public IViewSettings Settings { get; set; }

    /// <summary>Gets or sets the aspect ratio of the viewport, used for the projection matrix.</summary>
    public float AspectRatio { get; set; }

    /// <summary>Gets or sets the position of the camera.</summary>
    public Vector3 Position { get; set; }

    private protected bool firstMove;
    private protected Vector2 lastPos;

    /// <summary>
    ///     Gets the projection matrix of the view.
    /// </summary>
    /// <returns>The projection matrix.</returns>
    public virtual Matrix4x4 GetViewMatrix() => Matrix4x4.Identity;

    /// <summary>
    ///     Gets the view matrix of the view.
    /// </summary>
    /// <returns>The view matrix.</returns>
    public virtual Matrix4x4 GetProjectionMatrix() => Matrix4x4.Identity;

    /// <summary>
    ///     Sets the shader uniforms for the camera.
    /// </summary>
    /// <param name="shader">The shader to set the uniforms on.</param>
    public virtual void SetShaderUniforms(Shader shader)
    {
        shader.SetMatrix4("view", GetViewMatrix());
        shader.SetMatrix4("projection", GetProjectionMatrix());
        shader.SetVector3("viewPos", Position);
    }

    /// <summary>
    ///     Updates the camera's orientation based on the current mouse position.
    /// </summary>
    /// <param name="mousePosition">The current mouse position.</param>
    public virtual void UpdateMousePosition(Vector2 mousePosition)
    {
        if (firstMove)
        {
            lastPos = new Vector2(mousePosition.X, mousePosition.Y);
            firstMove = false;
        }
    }
}