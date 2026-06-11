using System;
using System.Reflection;

namespace SharpEngine.Core.Extensions;

/// <summary>
///     Contains extension methods for the <see cref="Assembly"/>.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    ///     Gets the version of the assembly as specified in its metadata.
    /// </summary>
    /// <param name="assembly">The assembly to get the version for.</param>
    /// <returns>The version of the assembly.</returns>
    /// <exception cref="NullReferenceException">Thrown when the assembly version is null.</exception>
    public static Version GetVersion(this Assembly assembly)
        => assembly.GetName().Version!;
}
