using OpenTK.Mathematics;

namespace Core;

/// <summary>
///     Represents a light source in the scene.
/// </summary>
public abstract class Light : GameObject
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Light"/>.
    /// </summary>
    protected Light()
    {
        Material.Shader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/shader.frag", "lamp");
    }

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

    /// <summary>
    ///     Renders the light using the specified shader.
    /// </summary>
    /// <param name="shader">The shader to use for rendering.</param>
    public virtual void Render(Shader shader) { }
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
    }

    /// <inheritdoc />
    public override void Render(Shader shader)
    {
        shader.SetVector3("dirLight.direction", Direction);
        shader.SetVector3("dirLight.ambient", Ambient);
        shader.SetVector3("dirLight.diffuse", Diffuse);
        shader.SetVector3("dirLight.specular", Specular);
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
        Scale = new(0.2f, 0.2f, 0.2f);
        Position = position;
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;

        _index = index;
    }

    private int _index;

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

    /// <summary>
    ///     Renders the point light using the specified shaders.
    /// </summary>
    /// <param name="lightingShader">The shader to use for lighting.</param>
    /// <param name="lampShader">The shader to use for the lamp.</param>
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
        CutOff = (float)MathHelper.Cos(MathHelper.DegreesToRadians(12.5f));
        OuterCutOff = (float)MathHelper.Cos(MathHelper.DegreesToRadians(15.0f));
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;
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
