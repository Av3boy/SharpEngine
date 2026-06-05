using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace SharpEngine.Core.Components.Properties.Meshes
{
    /// <summary>
    ///     Represents an OpenGL buffer object that manages memory allocation and data transfer for a specific value type.
    /// </summary>
    /// <typeparam name="TDataType">The unmanaged element type stored in the buffer.</typeparam>
    public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
    {
        private readonly uint _handle;
        private readonly BufferTargetARB _bufferType;
        private readonly GL _gl;
        private bool disposedValue;

        /// <summary>
        ///     Initializes a new instance of <see cref="BufferObject{TDataType}"/>.
        /// </summary>
        /// <param name="gl">The OpenGL context where the memory should be allocated.</param>
        /// <param name="data">The data buffer.</param>
        /// <param name="bufferType">The type of buffer target.</param>
        public BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;

            _handle = _gl.GenBuffer();
            Bind();

            var size = (nuint)(data.Length * Marshal.SizeOf<TDataType>());
            _gl.BufferData<TDataType>(bufferType, size, data, BufferUsageARB.StaticDraw);
        }

        /// <summary>
        ///     Binds the buffer object. 
        /// </summary>
        public void Bind() => _gl.BindBuffer(_bufferType, _handle);

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                _gl.DeleteBuffer(_handle);

            disposedValue = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
