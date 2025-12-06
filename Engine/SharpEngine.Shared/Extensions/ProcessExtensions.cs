using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpEngine.Shared.Extensions;

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
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/k {arguments}" : $"-k \"{arguments}\"",
                UseShellExecute = true,
                RedirectStandardOutput = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            }
        };

    /// <summary>
    ///     Runs the given commands using the terminal of the current platform.
    /// </summary>
    /// <param name="arguments">The commands to be executed.</param>
    /// <param name="createWindow">Determines whether the process should create a new window.</param>
    /// <param name="onMessage">The action that's invoked when a message is sent by the process handler.</param>
    public static void RunProcess(string arguments, bool createWindow = true, Action<string>? onMessage = null)
    {
        void SendMessage(string? msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
                onMessage?.Invoke(msg);
        }

        var process = GetProcess(arguments, createWindow);
        process.ErrorDataReceived += (sender, e) => SendMessage(e.Data);
        process.OutputDataReceived += (sender, e) => SendMessage(e.Data);
        process.Exited += (sender, e) => SendMessage("Process exited.");

        var start = new ThreadStart(() =>
        {
            try
            {
                process.Start();

                // Start reading output and error streams
                // process.BeginOutputReadLine();
                // process.BeginErrorReadLine();

                // Wait for the process to exit
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                SendMessage($"Exception occurred: {ex.Message}");
            }
        });

        var thread = new Thread(start);
        thread.Start();

        SendMessage("Editor launched.");
    }
}
