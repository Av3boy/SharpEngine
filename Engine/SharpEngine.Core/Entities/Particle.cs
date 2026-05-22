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

    public long StartTimeTicks { get; set; }
    public int LifeTimeMilliseconds { get; set; }
}
