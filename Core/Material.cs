using OpenTK.Mathematics;

namespace Core;

public class Material
{
    public Material()
    {
        Specular = new(0.5f, 0.5f, 0.5f);
        Shininess = 32.0f;
    }

    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }
    public Shader Shader { get; set; }

    internal readonly int diffuseUnit = 0;
    internal readonly int specularUnit = 1;

    public Vector3 Specular { get; set; }
    public float Shininess { get; set; }
}
