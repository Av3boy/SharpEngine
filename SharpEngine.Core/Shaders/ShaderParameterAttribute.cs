using SharpEngine.Core.Shaders.Rendering;
using System;

namespace SharpEngine.Core.Shaders;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ShaderParameterAttribute : Attribute
{
    public string Name { get; }
    public ShaderParameterType Type { get; }
    public ShaderUpdateFrequency Frequency { get; }

    public ShaderParameterAttribute(string name, ShaderParameterType type = ShaderParameterType.Unknown, ShaderUpdateFrequency frequency = ShaderUpdateFrequency.PerObject)
    {
        Name = name;
        Type = type;
        Frequency = frequency;
    }
}