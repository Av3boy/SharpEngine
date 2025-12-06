using Serilog;
using Serilog.Events;
using System;

namespace SharpEngine.Shared;

/// <summary>
///     Contains methods for debugging the application.
/// </summary>
public static class Debug
{
    public static ILogger Log;

    private static LogEventLevel _logLevel = LogEventLevel.Information;

    static Debug()
    {
        SetLogger();
    }

    public static void SetLogger()
        => Log = new LoggerConfiguration()
                    .MinimumLevel.Is(_logLevel)
                    .WriteTo.Console()
                    .CreateLogger();

    public static void SetLogLevel(LogEventLevel logLevel)
    {
        _logLevel = logLevel;
        SetLogger();
    }
}