using OpenTK.Mathematics;

namespace Core;

public abstract class Light : GameObject
{
    public Vector3 Ambient { get; set; }
    public Vector3 Diffuse { get; set; }
    public Vector3 Specular { get; set; }
}

public class DirectionalLight : Light
{
    public Vector3 Direction { get; set; }
}

public class PointLight : Light
{
    public PointLight()
    {
        Scale = new(0.2f, 0.2f, 0.2f);
    }

    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }
}

public class SpotLight : Light
{
    public Vector3 Direction { get; set; }
    public float CutOff { get; set; }
    public float OuterCutOff { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }
}
