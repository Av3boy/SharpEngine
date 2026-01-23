using System;
using System.Reflection;

namespace SharpEngine.Core.Extensions;

public static class AssemblyExtensions
{
    public static Version GetVersion(this Assembly assembly)
        => assembly.GetName().Version!;
}
