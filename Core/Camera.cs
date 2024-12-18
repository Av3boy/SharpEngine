using OpenTK.Mathematics;
using System;
using Plane = System.Numerics.Plane;

namespace Core;

/// <summary>
///     Represents a movable camera.
/// </summary>
/// <remarks>
///     Basically the implementation from <see href="https://github.com/opentk/LearnOpenTK"/>.
/// </remarks>
public class Camera
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Camera"/>.
    /// </summary>
    /// <param name="position">The initial position of the camera.</param>
    /// <param name="aspectRatio">The aspect ratio of the viewport.</param>
    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
    }

    // Those vectors are directions pointing outwards from the camera to define how it rotated.
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    /// <summary>Gets the front vector of the camera.</summary>
    public Vector3 Front => _front;

    /// <summary>Gets the up vector of the camera.</summary>
    public Vector3 Up => _up;

    /// <summary>Gets the right vector of the camera.</summary>
    public Vector3 Right => _right;

    // Rotation around the X axis (radians)
    private float _pitch;

    // Rotation around the Y axis (radians)
    private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;

    /// <summary>Gets or sets the position of the camera.</summary>
    public Vector3 Position { get; set; }

    /// <summary>Gets or sets the aspect ratio of the viewport, used for the projection matrix.</summary>
    public float AspectRatio { private get; set; }

    /// <summary>
    ///     Gets or sets the pitch (rotation around the X axis) of the camera in degrees.
    /// </summary>
    /// <remarks>
    ///     Convert from degrees to radians as soon as the property is set to improve performance.
    /// </remarks>
    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
            // of weird "bugs" when you are using euler angles for rotation.
            // If you want to read more about this you can try researching a topic called gimbal lock
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    /// <summary>
    ///     Gets or sets the yaw (rotation around the Y axis) of the camera in degrees.
    /// </summary>
    /// <remarks>
    ///     We convert from degrees to radians as soon as the property is set to improve performance.
    /// </remarks>
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    /// <summary>
    ///     Gets or sets the field of view (FOV) of the camera in degrees.
    /// </summary>
    /// <remarks>
    ///     The field of view (FOV) is the vertical angle of the camera view.
    ///     This has been discussed more in depth in a previous tutorial,
    ///     but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
    ///     We convert from degrees to radians as soon as the property is set to improve performance.
    /// </remarks>
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    /// <summary>
    ///     Gets the view matrix of the camera.
    /// </summary>
    /// <returns>The view matrix.</returns>
    public Matrix4 GetViewMatrix()
        => Matrix4.LookAt(Position, Position + _front, _up);

    /// <summary>
    ///     Gets the projection matrix of the camera.
    /// </summary>
    /// <returns>The projection matrix.</returns>
    public Matrix4 GetProjectionMatrix()
        => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);

    /// <summary>
    ///     Updates the direction vectors of the camera based on its current pitch and yaw.
    /// </summary>
    private void UpdateVectors()
    {
        // First, the front matrix is calculated using some basic trigonometry.
        _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

        // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
        _front = Vector3.Normalize(_front);

        // Calculate both the right and the up vector using cross product.
        // Note that we are calculating the right from the global up; this behaviour might
        // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }

    private bool firstMove;
    private Vector2 lastPos;

    // TODO: Move to the settings
    /// <summary>Gets or sets the sensitivity of the camera to mouse movements.</summary>
    public float Sensitivity { get; set; } = 0.2f;

    /// <summary>
    ///     Updates the camera's orientation based on the current mouse position.
    /// </summary>
    /// <param name="mousePosition">The current mouse position.</param>
    public void UpdateMousePosition(Vector2 mousePosition)
    {
        if (firstMove)
        {
            lastPos = new Vector2(mousePosition.X, mousePosition.Y);
            firstMove = false;
        }
        else
        {
            var deltaX = mousePosition.X - lastPos.X;
            var deltaY = mousePosition.Y - lastPos.Y;
            lastPos = new Vector2(mousePosition.X, mousePosition.Y);

            Yaw += deltaX * Sensitivity;
            Pitch -= deltaY * Sensitivity;
        }
    }

    /// <summary>
    ///     Gets the frustum planes of the camera.
    /// </summary>
    /// <returns>An array of six planes representing the camera's frustum.</returns>
    public Plane[] GetFrustumPlanes()
    {
        Matrix4 viewProjection = GetViewMatrix() * GetProjectionMatrix();
        Plane[] planes =
        [
            // Left
            new Plane(
                viewProjection.M14 + viewProjection.M11,
                viewProjection.M24 + viewProjection.M21,
                viewProjection.M34 + viewProjection.M31,
                viewProjection.M44 + viewProjection.M41),

            // Right
            new Plane(
                viewProjection.M14 - viewProjection.M11,
                viewProjection.M24 - viewProjection.M21,
                viewProjection.M34 - viewProjection.M31,
                viewProjection.M44 - viewProjection.M41),

            // Bottom
            new Plane(
                viewProjection.M14 + viewProjection.M12,
                viewProjection.M24 + viewProjection.M22,
                viewProjection.M34 + viewProjection.M32,
                viewProjection.M44 + viewProjection.M42),

            // Top
            new Plane(
                viewProjection.M14 - viewProjection.M12,
                viewProjection.M24 - viewProjection.M22,
                viewProjection.M34 - viewProjection.M32,
                viewProjection.M44 - viewProjection.M42),

            // Near
            new Plane(
                viewProjection.M13,
                viewProjection.M23,
                viewProjection.M33,
                viewProjection.M43),

            // Far
            new Plane(
                viewProjection.M14 - viewProjection.M13,
                viewProjection.M24 - viewProjection.M23,
                viewProjection.M34 - viewProjection.M33,
                viewProjection.M44 - viewProjection.M43),
        ];

        for (int i = 0; i < 6; i++)
            planes[i] = Plane.Normalize(planes[i]);

        return planes;
    }

    /// <summary>
    ///     Sets the shader uniforms for the camera.
    /// </summary>
    /// <param name="shader">The shader to set the uniforms on.</param>
    public void SetShaderUniforms(Shader shader)
    {
        shader.SetMatrix4("view", GetViewMatrix());
        shader.SetMatrix4("projection", GetProjectionMatrix());
        shader.SetVector3("viewPos", Position);
    }
}