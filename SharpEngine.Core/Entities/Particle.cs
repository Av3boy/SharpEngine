using System;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a particle emitted by a particle system.
/// </summary>
public class Particle : GameObject
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Particle"/> with a specified lifetime.
    /// </summary>
    /// <param name="lifeTimeMilliseconds">Determines how long the particle should stay on the screen.</param>
    public Particle(int lifeTimeMilliseconds)
    {
        LifeTimeMilliseconds = lifeTimeMilliseconds;
        StartTimeTicks = DateTime.UtcNow.Ticks;
    }

    /// <summary>
    ///     Gets or sets the time when the particle was emitted, represented in ticks.
    /// </summary>
    /// <remarks>
    ///     This value is used to determine when the particle should be removed based on its <seealso cref="LifeTimeMilliseconds">lifetime</seealso>.
    /// </remarks>
    public long StartTimeTicks { get; private set; }

    /// <summary>
    ///     Gets or sets the lifetime of the particle in milliseconds.
    /// </summary>
    /// <remarks>
    ///     This value is used to determine when the particle should be removed based on its <seealso cref="StartTimeTicks">start time</seealso>.
    /// </remarks>
    public int LifeTimeMilliseconds { get; private set; }
}
