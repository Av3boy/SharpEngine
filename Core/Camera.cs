using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using Plane = System.Numerics.Plane;

namespace Core;

// This is the camera class as it could be set up after the tutorials on the website.
// It is important to note there are a few ways you could have set up this camera.
// For example, you could have also managed the player input inside the camera class,
// and a lot of the properties could have been made into functions.

// TL;DR: This is just one of many ways in which we could have set up the camera.
// Check out the web version if you don't know why we are doing a specific thing or want to know more about the code.
public class Camera
{
    // Those vectors are directions pointing outwards from the camera to define how it rotated.
    private Vector3 _front = -Vector3.UnitZ;

    private Vector3 _up = Vector3.UnitY;

    private Vector3 _right = Vector3.UnitX;

    // Rotation around the X axis (radians)
    private float _pitch;

    // Rotation around the Y axis (radians)
    private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;

    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
    }

    // The position of the camera
    public Vector3 Position { get; set; }

    // This is simply the aspect ratio of the viewport, used for the projection matrix.
    public float AspectRatio { private get; set; }

    public Vector3 Front => _front;

    public Vector3 Up => _up;

    public Vector3 Right => _right;

    // We convert from degrees to radians as soon as the property is set to improve performance.
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

    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    // The field of view (FOV) is the vertical angle of the camera view.
    // This has been discussed more in depth in a previous tutorial,
    // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }

    // Get the projection matrix using the same method we have used up until this point
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    // This function is going to update the direction vertices using some of the math learned in the web tutorials.
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

    public float Sensitivity { get; set; } = 0.2f;

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

    public Plane[] GetFrustumPlanes()
    {
        Matrix4 viewProjection = GetViewMatrix() * GetProjectionMatrix();
        Plane[] planes = new Plane[6];

        // Left
        planes[0] = new Plane(
            viewProjection.M14 + viewProjection.M11,
            viewProjection.M24 + viewProjection.M21,
            viewProjection.M34 + viewProjection.M31,
            viewProjection.M44 + viewProjection.M41);

        // Right
        planes[1] = new Plane(
            viewProjection.M14 - viewProjection.M11,
            viewProjection.M24 - viewProjection.M21,
            viewProjection.M34 - viewProjection.M31,
            viewProjection.M44 - viewProjection.M41);

        // Bottom
        planes[2] = new Plane(
            viewProjection.M14 + viewProjection.M12,
            viewProjection.M24 + viewProjection.M22,
            viewProjection.M34 + viewProjection.M32,
            viewProjection.M44 + viewProjection.M42);

        // Top
        planes[3] = new Plane(
            viewProjection.M14 - viewProjection.M12,
            viewProjection.M24 - viewProjection.M22,
            viewProjection.M34 - viewProjection.M32,
            viewProjection.M44 - viewProjection.M42);

        // Near
        planes[4] = new Plane(
            viewProjection.M13,
            viewProjection.M23,
            viewProjection.M33,
            viewProjection.M43);

        // Far
        planes[5] = new Plane(
            viewProjection.M14 - viewProjection.M13,
            viewProjection.M24 - viewProjection.M23,
            viewProjection.M34 - viewProjection.M33,
            viewProjection.M44 - viewProjection.M43);

        for (int i = 0; i < 6; i++)
        {
            planes[i] = Plane.Normalize(planes[i]);
        }

        return planes;
    }

    public void SetShaderUniforms(Shader shader)
    {
        shader.SetMatrix4("view", GetViewMatrix());
        shader.SetMatrix4("projection", GetProjectionMatrix());
        shader.SetVector3("viewPos", Position);
    }
}