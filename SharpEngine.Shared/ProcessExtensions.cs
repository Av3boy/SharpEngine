using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpEngine.Shared;

public class ProcessExtensions
{
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
