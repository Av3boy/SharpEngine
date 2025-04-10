namespace SharpEngine.Core.Components.Properties.Meshes.MeshData.VertexData
{
    public struct TextureCoordinate
    {
        public TextureCoordinate(float x, float y) : this()
        {
            X = x;
            Y = y;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
    }
}