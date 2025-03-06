using Core.Entities;
using System.Numerics;

namespace SharpEngine.Core.Entities.Lights;

/// <summary>
///     Represents a light source in the scene.
/// </summary>
public abstract class Light : GameObject
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Light"/>.
    /// </summary>
    protected Light() { }

    /// <summary>
    ///     Gets or sets the ambient color of the light.
    /// </summary>
    public Vector3 Ambient { get; set; } = new Vector3(0.05f, 0.05f, 0.05f);

    /// <summary>
    ///     Gets or sets the diffuse color of the light.
    /// </summary>
    public Vector3 Diffuse { get; set; } = new Vector3(0.8f, 0.8f, 0.8f);

    /// <summary>
    ///     Gets or sets the specular color of the light.
    /// </summary>
    public Vector3 Specular { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
}
