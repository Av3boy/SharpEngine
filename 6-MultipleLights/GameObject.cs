using LearnOpenTK.Common;
using OpenTK.Mathematics;

namespace LearnOpenTK;
public class GameObject : SceneNode
{
    public Mesh Mesh { get; set; }
    public Vector3 Position { get; set; }
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }
    public Shader Shader { get; set; }
    public Quaternion Quaternion { get; set; } = new();
}

public class Quaternion
{
    public float Angle { get; set; }
    public Vector3 Axis { get; set; } = new(0, 1, 0);
}
