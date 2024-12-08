using OpenTK.Mathematics;

namespace Core;

public class Material
{
    public Material()
    {
        Specular = new(0.5f, 0.5f, 0.5f);
        Shininess = 32.0f;
    }

    internal readonly int diffuseUnit = 0;
    internal readonly int specularUnit = 1;

    public Vector3 Specular { get; set; }
    public float Shininess { get; set; }
}
