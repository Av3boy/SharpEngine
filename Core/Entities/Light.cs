using Core.Shaders;
using System.Numerics;
using System.Threading.Tasks;

namespace Core.Entities;

/// <summary>
///     Represents a light source in the scene.
/// </summary>
public abstract class Light : GameObject
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Light"/>.
    /// </summary>
    protected Light() { }

    /// <summary>
    ///     Gets or sets the ambient color of the light.
    /// </summary>
    public Vector3 Ambient { get; set; } = new Vector3(0.05f, 0.05f, 0.05f);

    /// <summary>
    ///     Gets or sets the diffuse color of the light.
    /// </summary>
    public Vector3 Diffuse { get; set; } = new Vector3(0.8f, 0.8f, 0.8f);

    /// <summary>
    ///     Gets or sets the specular color of the light.
    /// </summary>
    public Vector3 Specular { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
}

/// <summary>
///     Represents a directional light source.
/// </summary>
public class DirectionalLight : Light
{
    /// <summary>
    ///     Gets or sets the direction of the light.
    /// </summary>
    public Vector3 Direction { get; set; } = new Vector3(-0.2f, -1.0f, -0.3f);

    /// <summary>
    ///     Initializes a new instance of <see cref="DirectionalLight"/>.
    /// </summary>
    public DirectionalLight()
    {
        Diffuse = new Vector3(0.4f, 0.4f, 0.4f);
        Specular = new Vector3(0.5f, 0.5f, 0.5f);

        Material.Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
    }

    /// <inheritdoc />
    public override Task Render()
    {
        Material.Shader.SetVector3("dirLight.direction", Direction);
        Material.Shader.SetVector3("dirLight.ambient", Ambient);
        Material.Shader.SetVector3("dirLight.diffuse", Diffuse);
        Material.Shader.SetVector3("dirLight.specular", Specular);

        return Task.CompletedTask;
    }
}

/// <summary>
///     Represents a point light source.
/// </summary>
public class PointLight : Light
{
    /// <summary>
    ///     Initializes a new instance of <see cref="PointLight"/>.
    /// </summary>
    /// <param name="position">The position of the light.</param>
    /// <param name="index">The index of the light in the shader array.</param>
    public PointLight(Vector3 position, int index)
    {
        Transform.Scale = new(0.2f, 0.2f, 0.2f);
        Transform.Position = position;
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;

        _index = index;

        Material.Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
        LampShader = new LampShader();

    }

    private LampShader LampShader { get; set; }

    private readonly int _index;

    /// <summary>
    ///     Gets or sets the constant attenuation factor.
    /// </summary>
    public float Constant { get; set; }

    /// <summary>
    ///     Gets or sets the linear attenuation factor.
    /// </summary>
    public float Linear { get; set; }

    /// <summary>
    ///     Gets or sets the quadratic attenuation factor.
    /// </summary>
    public float Quadratic { get; set; }

    /// <inheritdoc />
    public override Task Render()
    {
        Material.Shader.SetVector3($"pointLights[{_index}].position", Transform.Position);
        Material.Shader.SetVector3($"pointLights[{_index}].ambient", Ambient);
        Material.Shader.SetVector3($"pointLights[{_index}].diffuse", Diffuse);
        Material.Shader.SetVector3($"pointLights[{_index}].specular", Specular);
        Material.Shader.SetFloat($"pointLights[{_index}].constant", Constant);
        Material.Shader.SetFloat($"pointLights[{_index}].linear", Linear);
        Material.Shader.SetFloat($"pointLights[{_index}].quadratic", Quadratic);

        var lampMatrix = Matrix4x4.CreateScale(Transform.Scale);
        lampMatrix *= Matrix4x4.CreateTranslation(Transform.Position);

        LampShader.Shader.SetMatrix4(ShaderAttributes.Model, lampMatrix);

        return Task.CompletedTask;
    }
}

/// <summary>
///     Represents a spotlight source.
/// </summary>
public class SpotLight : Light
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SpotLight"/> class.
    /// </summary>
    public SpotLight()
    {
        Direction = new Vector3(0.0f, -1.0f, 0.0f);
        CutOff = (float)System.Math.Cos(Math.DegreesToRadians(12.5f));
        OuterCutOff = (float)System.Math.Cos(Math.DegreesToRadians(15.0f));
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;

        Material.Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
    }

    /// <summary>
    ///     Gets or sets the direction of the spotlight.
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    ///     Gets or sets the cutoff angle of the spotlight.
    /// </summary>
    public float CutOff { get; set; }

    /// <summary>
    ///     Gets or sets the outer cutoff angle of the spotlight.
    /// </summary>
    public float OuterCutOff { get; set; }

    /// <summary>
    ///     Gets or sets the constant attenuation factor.
    /// </summary>
    public float Constant { get; set; }

    /// <summary>
    ///     Gets or sets the linear attenuation factor.
    /// </summary>
    public float Linear { get; set; }

    /// <summary>
    ///     Gets or sets the quadratic attenuation factor.
    /// </summary>
    public float Quadratic { get; set; }

    /// <inheritdoc />
    public override Task Render()
    {
        Material.Shader.SetVector3("spotLight.position", Transform.Position);
        Material.Shader.SetVector3("spotLight.direction", Direction);
        Material.Shader.SetVector3("spotLight.ambient", Ambient);
        Material.Shader.SetVector3("spotLight.diffuse", Diffuse);
        Material.Shader.SetVector3("spotLight.specular", Specular);
        Material.Shader.SetFloat("spotLight.constant", Constant);
        Material.Shader.SetFloat("spotLight.linear", Linear);
        Material.Shader.SetFloat("spotLight.quadratic", Quadratic);
        Material.Shader.SetFloat("spotLight.cutOff", CutOff);
        Material.Shader.SetFloat("spotLight.outerCutOff", OuterCutOff);

        return Task.CompletedTask;
    }
}
