using System;
using System.Collections.Generic;

namespace SharpEngine.Core.Rendering
{
    public enum ParameterType
    {
        Float,
        Vec2,
        Vec3,
        Vec4,
        Mat4,
        Texture,
        Unknown
    }

    public enum UpdateFrequency
    {
        Static,
        PerObject,
        PerFrame
    }

    public sealed class ParameterDescriptor
    {
        public string Name { get; }
        public ParameterType Type { get; }
        public UpdateFrequency Frequency { get; }

        public ParameterDescriptor(string name, ParameterType type, UpdateFrequency frequency = UpdateFrequency.PerObject)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            Frequency = frequency;
        }
    }

    public sealed class ParameterInstance
    {
        public ParameterDescriptor Descriptor { get; }

        // Boxed value. Use typed accessors/helpers in ShaderProgram to avoid frequent casts.
        private object _value;
        public object Value => _value;

        public bool Dirty { get; private set; }

        // Used to avoid re-uploading per-frame params multiple times per frame
        public int LastUploadedFrame { get; internal set; } = -1;

        public ParameterInstance(ParameterDescriptor descriptor, object initialValue = null)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            _value = initialValue;
            Dirty = true; // not uploaded yet
        }

        public bool SetValue(object newValue, bool forceDirty = false)
        {
            if (!forceDirty && Equals(_value, newValue))
                return false;

            _value = newValue;
            Dirty = true;
            return true;
        }

        public void ClearDirty() => Dirty = false;
    }
}
