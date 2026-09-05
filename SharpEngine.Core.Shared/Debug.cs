using Serilog;
using Serilog.Events;
using System;
using System.Numerics;

namespace SharpEngine.Shared;

/// <summary>
///     Contains methods for debugging the application.
/// </summary>
public static class Debug
{
    /// <summary>
    ///     A logger instance used for debugging the application.
    /// </summary>
    public static ILogger Log { get; set; }

    private static LogEventLevel _logLevel = LogEventLevel.Information;

    /// <summary>
    ///     Initializes the <see cref="Debug" /> class instance.
    /// </summary>
    static Debug()
    {
        SetLogger();
    }

    /// <summary>
    ///     Updates the logger instance.
    /// </summary>
    public static void SetLogger()
        => Log = new LoggerConfiguration()
                    .MinimumLevel.Is(_logLevel)
                    .WriteTo.Console()
                    .CreateLogger();

    /// <summary>
    ///     Sets the logging level for the application.
    /// </summary>
    /// <remarks>
    ///     This affects the granularity of log messages generated.
    /// </remarks>
    /// <param name="logLevel">Specifies the severity level of log messages to be recorded.</param>
    public static void SetLogLevel(LogEventLevel logLevel)
    {
        _logLevel = logLevel;
        SetLogger();
    }

#if DEBUG

    /// <summary>
    ///     Draws a line on the screen.
    /// </summary>
    /// <param name="startPoint">The starting point for the line.</param>
    /// <param name="direction">The direction where the vector needs to be drawn.</param>
    /// <param name="length">Determines how long the line needs to be.</param>
    /// <param name="width">Determines how wide of a vector needs to be drawn.</param>
    /// <exception cref="NotImplementedException">Thrown when the method is called, indicating that the implementation is not yet provided.</exception>
    public static void DrawLine(Vector3 startPoint, Vector3 direction, float length, float width = 1)
    {
        // https://math.stackexchange.com/questions/1286489/how-to-find-direction-and-normal-vector
        throw new NotImplementedException();
    }

    /// <summary>
    /// Draws a box in 3D space defined by two corner points.
    /// </summary>
    /// <param name="min">Specifies one corner of the box in 3D coordinates.</param>
    /// <param name="max">Specifies the opposite corner of the box in 3D coordinates.</param>
    /// <param name="wireframe">Determines whether the box is rendered as a solid shape or as a wireframe.</param>
    /// <exception cref="NotImplementedException">Thrown when the method has not been implemented yet.</exception>
    public static void DrawBox(Vector3 min, Vector3 max, bool wireframe = true)
    {
        throw new NotImplementedException();
    }

#endif
}