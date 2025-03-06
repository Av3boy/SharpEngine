using System.Numerics;

namespace SharpEngine.Core.Entities.Properties;

/// <summary>
///     Represents a game object transformation in 2D space.
/// </summary>
public class Transform2D
{
    /// <summary>
    ///     Gets or sets the position of the game object.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    ///     Gets or sets the scale of the game object.
    /// </summary>
    public Vector2 Scale { get; set; } = new(1, 1);

    /// <summary>
    ///     Gets or sets the rotation of the game object.
    /// </summary>
    public float Rotation { get; set; }

    /// <summary>
    ///     Gets the transformation of the game object as a model matrix.
    /// </summary>
    public Matrix4x4 ModelMatrix => Matrix4x4.CreateScale(new Vector3(Scale.X, Scale.Y, 0)) *
                                    Matrix4x4.CreateRotationZ(Rotation) *
                                    Matrix4x4.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
}