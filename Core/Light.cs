using OpenTK.Mathematics;
using System;

namespace Core;

public abstract class Light : GameObject
{
    protected Light()
    {
        Material.Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag", "lamp");
    }

    public Vector3 Ambient { get; set; } = new Vector3(0.05f, 0.05f, 0.05f);
    public Vector3 Diffuse { get; set; } = new Vector3(0.8f, 0.8f, 0.8f);
    public Vector3 Specular { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);

    public virtual void Render(Shader shader) { }
}

public class DirectionalLight : Light
{
    public Vector3 Direction { get; set; } = new Vector3(-0.2f, -1.0f, -0.3f);

    public DirectionalLight()
    {
        Diffuse = new Vector3(0.4f, 0.4f, 0.4f);
        Specular = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public override void Render(Shader shader)
    {
        shader.SetVector3("dirLight.direction", Direction);
        shader.SetVector3("dirLight.ambient", Ambient);
        shader.SetVector3("dirLight.diffuse", Diffuse);
        shader.SetVector3("dirLight.specular", Specular);
    }
}

public class PointLight : Light
{
    public PointLight(Vector3 position, int index)
    {
        Scale = new(0.2f, 0.2f, 0.2f);
        Position = position;
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;

        _index = index;
    }

    private int _index;

    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }

    public void Render(Shader lightingShader, Shader lampShader)
    {
        lightingShader.SetVector3($"pointLights[{_index}].position", Position);
        lightingShader.SetVector3($"pointLights[{_index}].ambient", Ambient);
        lightingShader.SetVector3($"pointLights[{_index}].diffuse", Diffuse);
        lightingShader.SetVector3($"pointLights[{_index}].specular", Specular);
        lightingShader.SetFloat($"pointLights[{_index}].constant", Constant);
        lightingShader.SetFloat($"pointLights[{_index}].linear", Linear);
        lightingShader.SetFloat($"pointLights[{_index}].quadratic", Quadratic);

        Matrix4 lampMatrix = Matrix4.CreateScale(Scale);
        lampMatrix *= Matrix4.CreateTranslation(Position);

        lampShader.SetMatrix4("model", lampMatrix);
    }
}

public class SpotLight : Light
{
    public SpotLight()
    {
        Direction = new Vector3(0.0f, -1.0f, 0.0f);
        CutOff = (float)MathHelper.Cos(MathHelper.DegreesToRadians(12.5f));
        OuterCutOff = (float)MathHelper.Cos(MathHelper.DegreesToRadians(15.0f));
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;
    }

    public Vector3 Direction { get; set; }
    public float CutOff { get; set; }
    public float OuterCutOff { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }

    public override void Render(Shader shader)
    {
        shader.SetVector3("spotLight.position", Position);
        shader.SetVector3("spotLight.direction", Direction);
        shader.SetVector3("spotLight.ambient", Ambient);
        shader.SetVector3("spotLight.diffuse", Diffuse);
        shader.SetVector3("spotLight.specular", Specular);
        shader.SetFloat("spotLight.constant", Constant);
        shader.SetFloat("spotLight.linear", Linear);
        shader.SetFloat("spotLight.quadratic", Quadratic);
        shader.SetFloat("spotLight.cutOff", CutOff);
        shader.SetFloat("spotLight.outerCutOff", OuterCutOff);
    }
}
