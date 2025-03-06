using Core.Shaders;
using SharpEngine.Core.Entities.Views.Settings;
using System.Numerics;

namespace SharpEngine.Core.Entities.Views;

public class View
{
    public View(IViewSettings settings)
    {
        Settings = settings;
    }

    public IViewSettings Settings { get; set; }

    /// <summary>Gets or sets the aspect ratio of the viewport, used for the projection matrix.</summary>
    public float AspectRatio { get; set; }

    /// <summary>Gets or sets the position of the camera.</summary>
    public Vector3 Position { get; set; }

    protected bool firstMove;
    protected Vector2 lastPos;

    public virtual Matrix4x4 GetViewMatrix() => Matrix4x4.Identity;
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
        else
            lastPos = new Vector2(mousePosition.X, mousePosition.Y);
    }
}