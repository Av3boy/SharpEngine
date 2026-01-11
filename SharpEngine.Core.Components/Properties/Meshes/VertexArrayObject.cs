using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    /// <summary>
    ///     Represents a vertex array object for managing vertex and index data in OpenGL.
    /// </summary>
    /// <typeparam name="TVertexType">Specifies the type of vertex data used in the vertex array.</typeparam>
    /// <typeparam name="TIndexType">Specifies the type of index data used for indexing vertices in the vertex array.</typeparam>
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private readonly uint _handle;
        private readonly GL _gl;
        private bool disposedValue;

        /// <summary>
        ///     Initializes an instance of <see cref="VertexArrayObject{TVertexType, TIndexType}" />.
        /// </summary>
        /// <param name="gl">Represents the OpenGL context used for generating and managing the vertex array.</param>
        /// <param name="vbo">Represents the buffer object containing vertex data that will be bound to the vertex array.</param>
        /// <param name="ebo">Represents the buffer object containing index data that will be bound to the vertex array.</param>
        public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
        {
            _gl = gl;

            _handle = _gl.GenVertexArray();
            Bind();
            vbo.Bind();
            ebo.Bind();
        }

        /// <summary>
        ///     Configures the vertex attribute pointer for rendering.
        /// </summary>
        /// <param name="index">Specifies the index of the vertex attribute to be configured.</param>
        /// <param name="count">Indicates the number of components per vertex attribute.</param>
        /// <param name="type">Defines the data type of each component in the vertex attribute.</param>
        /// <param name="vertexSize">Determines the size of each vertex in bytes.</param>
        /// <param name="offSet">Sets the offset in bytes from the start of the vertex data.</param>
        public void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            var size = Marshal.SizeOf<TVertexType>();
            var stride = (uint)(vertexSize * size);
            var pointer = (nint)offSet * size;

            _gl.VertexAttribPointer(index, count, type, false, stride, pointer);
            _gl.EnableVertexAttribArray(index);
        }

        /// <summary>
        ///     Binds the vertex array object (VAO) to the current OpenGL context.
        /// </summary>
        public void Bind() 
            => _gl.BindVertexArray(_handle);

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            
            if (disposing)
                _gl.DeleteVertexArray(_handle);

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
