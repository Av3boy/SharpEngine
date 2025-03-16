using System;
using System.IO;

namespace SharpEngine.Core.Extensions;

/// <summary>
///     Contains extensions for handling paths and files.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    ///     Gets the path for the file in the executing assembly directory.
    /// </summary>
    /// <param name="file">The file whose path is being resolved.</param>
    /// <returns>The file in the currently executing assembly context.</returns>
    public static string GetAssemblyPath(string file)
        => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
}
