namespace SharpEngine.Core.Windowing;

/// <summary>
///     Represents a frame with a specific duration in time.
/// </summary>
public class Frame
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Frame" />.
    /// </summary>
    /// <param name="frameTime">Represents the duration of the frame in seconds.</param>
    public Frame(double frameTime)
    {
        FrameTime = frameTime;
    }

    /// <summary>Gets or sets the time taken to handle the previous frame.</summary>
    public double FrameTime { get; set; }

    /// <summary>Gets the frame rate based on the frame time.</summary>
    public float FrameRate => (float)(1 / FrameTime);
}
