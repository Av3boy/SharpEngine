using Serilog;
using System;

namespace SharpEngine.Core;

/// <summary>
///     Contains methods for debugging the application.
/// </summary>
public static class Debug
{
    /// <summary>
    ///     Logs an information message.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="ex">The possible exception.</param>
    /// <param name="writeToFile">Determines whether the log should be written to a file.</param>
    public static void LogInformation(string message, Exception? ex = null, bool writeToFile = false)
    {
        Console.WriteLine(message);

        if (!writeToFile)
            return;

        var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        log.Information(ex, message);
    }
}