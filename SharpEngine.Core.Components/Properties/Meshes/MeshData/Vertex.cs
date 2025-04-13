namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    // TODO: Use vertex2 instead of this one

    public struct Vertex
    {
        public Vertex(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
    }
}