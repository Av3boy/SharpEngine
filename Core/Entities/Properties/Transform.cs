using Silk.NET.Maths;
using SharpEngine.Core.Entities;
using System.Numerics;

using Vector3 = SharpEngine.Core.Numerics.Vector3;

namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents a game object transformation in 3D space.
/// </summary>
public class Transform : ITransform<Vector3>
{
    public Transform()
    {
    }

    /// <summary>
    ///     Gets or sets the position of the game object.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    ///     Gets or sets the scale of the game object.
    /// </summary>
    public Vector3 Scale { get; set; } = new(1, 1, 1);

    /// <summary>
    ///     Gets or sets the rotation of the game object.
    /// </summary>
    public Quaternion Rotation { get; set; } = new();

    /// <summary>
    ///     Gets the transformation of the game object as a model matrix.
    /// </summary>
    public Matrix4x4 ModelMatrix => Matrix4x4.CreateScale(new System.Numerics.Vector3(Scale.X, Scale.Y, Scale.Z)) *
                                  Matrix4x4.CreateFromAxisAngle(Rotation.Axis, Math.DegreesToRadians(Rotation.Angle)) *
                                  Matrix4x4.CreateTranslation(new System.Numerics.Vector3(Position.X, Position.Y, Position.Z));
}
