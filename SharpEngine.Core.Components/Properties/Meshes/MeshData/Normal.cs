namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    /// <summary>
    ///     Represents a normal vector in 3D space, which is used in mesh data to define the direction perpendicular to the surface of a triangle, affecting how light interacts with the surface for rendering purposes.
    /// </summary>
    public struct Normal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Normal"/> struct with the specified x, y, and z components, representing the normal vector's direction in 3D space.
        /// </summary>
        /// <param name="x">The x-component of the normal vector.</param>
        /// <param name="y">The y-component of the normal vector.</param>
        /// <param name="z">The z-component of the normal vector.</param>
        public Normal(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Gets the x-component of the normal vector, representing the horizontal direction in 3D space.
        /// </summary>
        public float X { get; private set; }
        
        /// <summary>
        ///     Gets the y-component of the normal vector, representing the vertical direction in 3D space.
        /// </summary>
        public float Y { get; private set; }
        
        /// <summary>
        ///     Gets the z-component of the normal vector, representing the depth direction in 3D space.
        /// </summary>
        public float Z { get; private set; }
    }
}