using OpenTK.Mathematics;

namespace Core;

public class GameObject : SceneNode
{
    public GameObject() { }

    public GameObject(string diffuseMapFile, string specularMapFile)
    {
        DiffuseMap = TextureService.Instance.LoadTexture(diffuseMapFile);
        SpecularMap = TextureService.Instance.LoadTexture(specularMapFile);
    }

    public Mesh Mesh { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; } = new(1, 1, 1);
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }
    public Shader Shader { get; set; }
    public Quaternion Quaternion { get; set; } = new();
    public Material Material { get; set; } = new();
}

public class Quaternion
{
    public float Angle { get; set; }
    public Vector3 Axis { get; set; } = new(0, 1, 0);
}
