using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Entities.Properties.Meshes;

namespace SharpEngine.Core.Entities.UI;

public class MeshRenderer
{
    public MeshRenderer(Mesh mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }

    // TODO: This should be the correct structure for the MeshRenderer, but for now we will keep these separate
    public MeshRenderer(Model model, Material material)
    {
        Model = model;
        Material = material;
    }

    public Model Model { get; }
    public Mesh Mesh { get; }
    public Material Material { get; }
}