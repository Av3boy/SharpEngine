using OpenTK.Mathematics;
using System;

namespace Core;

public abstract class Light : GameObject
{
    public Light()
    {
        Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag");
    }

    public Vector3 Ambient { get; set; } = new Vector3(0.05f, 0.05f, 0.05f);
    public Vector3 Diffuse { get; set; }
    public Vector3 Specular { get; set; }
}

public class DirectionalLight : Light
{
    public Vector3 Direction { get; set; } = new Vector3(-0.2f, -1.0f, -0.3f);

    public DirectionalLight()
    {
        Diffuse = new Vector3(0.4f, 0.4f, 0.4f);
        Specular = new Vector3(0.5f, 0.5f, 0.5f);
    }
}

public class PointLight : Light
{
    public PointLight(Vector3 position)
    {
        Scale = new(0.2f, 0.2f, 0.2f);
        Position = position;
        Ambient = new Vector3(0.05f, 0.05f, 0.05f);
        Diffuse = new Vector3(0.8f, 0.8f, 0.8f);
        Specular = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public float Constant { get; set; } = 1.0f;
    public float Linear { get; set; } = 0.09f;
    public float Quadratic { get; set; } = 0.032f;
}

public class SpotLight : Light
{
    public Vector3 Direction { get; set; }
    public float CutOff { get; set; } = MathF.Cos(MathHelper.DegreesToRadians(12.5f));
    public float OuterCutOff { get; set; } = MathF.Cos(MathHelper.DegreesToRadians(17.5f));
    public float Constant { get; set; } = 1.0f;
    public float Linear { get; set; } = 0.09f;
    public float Quadratic { get; set; } = 0.032f;
}
