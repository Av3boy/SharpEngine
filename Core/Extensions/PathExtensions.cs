using System;
using System.IO;
using System.Reflection;

namespace SharpEngine.Core.Extensions;
public static class PathExtensions
{
    public static string GetPath(string file)
        => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
}
