using SharpEngine.Core.Components.Properties.Meshes.MeshData;

namespace SharpEngine.Core.Components.ObjLoader.DataStore
{
    public interface ITextureDataStore
    {
        void AddTexture(TextureCoordinate texture);
    }
}