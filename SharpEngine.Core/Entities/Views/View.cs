using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using System;
using System.Collections.Generic;
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

    protected Vector3 _front = -Vector3.UnitZ;
    protected Vector3 _up = Vector3.UnitY;
    protected Vector3 _right = Vector3.UnitX;

    /// <summary>Gets the front vector of the camera.</summary>
    public Vector3 Front => _front;

    /// <summary>Gets the up vector of the camera.</summary>
    public Vector3 Up => _up;

    /// <summary>Gets the right vector of the camera.</summary>
    public Vector3 Right => _right;

        /// <summary>Gets or sets the position of the camera.</summary>
    public Vector3 Position { get; set; }

    /// <summary>Gets or sets the settings for the view.</summary>
    public IViewSettings Settings { get; set; }

    /// <summary>Gets or sets the aspect ratio of the viewport, used for the projection matrix.</summary>
    public float AspectRatio { get; set; }

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

    /// <summary>
    ///     Checks whether a game object is in view of the camera.
    /// </summary>
    /// <remarks>
    ///     Uses a step based ray marching algorithm to check if any object is in view of the camera.
    /// </remarks>
    /// <param name="scene">The scene where the check should be applied to.</param>
    /// <param name="allowedTypes">The types that are allowed to be hit.</param>
    /// <param name="intersectingObject">The first object that the ray intersected.</param>
    /// <param name="hitPosition">The position where the object was hit.</param>
    /// <returns><see langword="true"/> if any object was hit; otherwise, <see langword="false"/>.</returns>
    public bool IsInView(Scene scene, out GameObject? intersectingObject, out Vector3 hitPosition, params Type[] allowedTypes)
        => new Ray(Position, Front).IsGameObjectInView(scene, out intersectingObject, out hitPosition, allowedTypes);
}