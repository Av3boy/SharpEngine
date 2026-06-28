using System;

namespace SharpEngine.Core.Rendering
{
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
