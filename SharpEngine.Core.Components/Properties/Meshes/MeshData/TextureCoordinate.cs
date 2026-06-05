namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    /// <summary>
    ///     Represents a texture coordinate, which is used to map textures onto the surface of a 3D model.
    ///     Each texture coordinate consists of two components: X and Y, which range from 0 to 1, where (0,0) corresponds to the bottom-left corner of the texture and (1,1) corresponds to the top-right corner.
    /// </summary>
    public struct TextureCoordinate
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureCoordinate"/> struct with the specified X and Y values.
        /// </summary>
        /// <param name="x">The X-component of the texture coordinate.</param>
        /// <param name="y">The Y-component of the texture coordinate.</param>
        public TextureCoordinate(float x, float y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Gets the X-component of the texture coordinate, which represents the horizontal position on the texture map.
        /// </summary>
        /// <remarks>
        ///     The value ranges from 0 to 1, where 0 corresponds to the left edge of the texture and 1 corresponds to the right edge.
        /// </remarks>
        public float X { get; private set; }

        /// <summary>
        ///     Gets the Y-component of the texture coordinate, which represents the vertical position on the texture map.
        /// </summary>
        /// <remarks>
        ///     The value ranges from 0 to 1, where 0 corresponds to the bottom edge of the texture and 1 corresponds to the top edge.
        /// </remarks>
        public float Y { get; private set; }
    }
}