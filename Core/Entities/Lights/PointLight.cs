using SharpEngine.Core.Shaders;
using System.Numerics;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.Lights;

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

        LampShader.Shader?.SetMatrix4(ShaderAttributes.Model, lampMatrix);

        return Task.CompletedTask;
    }
}
