using System;

namespace SharpEngine.Core.Rendering
{
    public sealed class ParameterDescriptor
    {
        public string Name { get; }
        public ShaderParameterType Type { get; }
        public ShaderUpdateFrequency Frequency { get; }

        public ParameterDescriptor(string name, ShaderParameterType type, ShaderUpdateFrequency frequency = ShaderUpdateFrequency.PerObject)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            Frequency = frequency;
        }
    }
}
