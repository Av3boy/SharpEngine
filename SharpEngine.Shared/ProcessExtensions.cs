using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpEngine.Shared;

/// <summary>
///     Contains extensions for handling processes.
/// </summary>
public class ProcessExtensions
{
    /// <summary>
    ///     Creates a new process to execute a command with specified arguments.
    /// </summary>
    /// <param name="arguments">Specifies the command-line arguments to be passed to the process.</param>
    /// <param name="createWindow">Indicates whether to create a new window for the process or not.</param>
    /// <returns>
    ///     Returns a new <see cref="Process"/> instance configured with the provided arguments.
    /// </returns>
    public static Process GetProcess(string arguments, bool createWindow = false)
        => new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = createWindow
            }
        };
}
