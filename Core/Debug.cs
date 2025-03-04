using Serilog;
using System;

namespace SharpEngine.Core
{
    public static class Debug
    {
        public static void LogInformation(string message, Exception? ex = null, bool writeToFile = false)
        {
            Console.WriteLine(message);

            if (!writeToFile)
                return;

            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            log.Information(ex, message);
        }
    }
}