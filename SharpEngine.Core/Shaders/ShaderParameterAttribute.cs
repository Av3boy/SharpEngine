using SharpEngine.Core.Rendering;
using System;

namespace SharpEngine.Core.Shaders;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ShaderParameterAttribute : Attribute
{
    public string Name { get; }
    public ParameterType Type { get; }
    public UpdateFrequency Frequency { get; }

    public ShaderParameterAttribute(string name, ParameterType type = ParameterType.Unknown, UpdateFrequency frequency = UpdateFrequency.PerObject)
    {
        Name = name;
        Type = type;
        Frequency = frequency;
    }
}