using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Shared.Extensions;
public static class ReflectionExtensions
{
    public static object? GetPropertyValue<T>(this T obj, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
            return null;

        return property.GetValue(obj);
    }
}
