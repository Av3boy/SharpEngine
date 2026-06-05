using System.Numerics;
using Vector2 = SharpEngine.Core.Numerics.Vector2;

namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents a game object transformation in 2D space.
/// </summary>
public class Transform2D : ITransform<Vector2>
{
    /// <summary>
    ///     Gets or sets the position of the game object.
    /// </summary>
    public Vector2 Position { get; set; } = new(0, 0);

    /// <summary>
    ///     Gets or sets the scale of the game object.
    /// </summary>
    public Vector2 Scale { get; set; } = new(1, 1);

    /// <summary>
    ///     Gets or sets the rotation of the game object.
    /// </summary>
    public Quaternion Rotation { get; set; } = new() { Angle = 0 };

    /// <summary>
    ///     Gets the transformation of the game object as a model matrix.
    /// </summary>
    public Matrix4x4 ModelMatrix => Matrix4x4.CreateScale(new Vector3(Scale.X, Scale.Y, 0)) *
                                    // Matrix4x4.CreateRotationZ(Math.DegreesToRadians(Rotation.Angle)) *
                                    Matrix4x4.CreateRotationZ(0) *
                                    // Matrix4x4.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
                                    Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
}