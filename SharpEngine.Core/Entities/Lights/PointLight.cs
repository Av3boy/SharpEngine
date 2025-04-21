using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
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
        Transform.Position = (Numerics.Vector3)position;
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;

        _index = index;

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

    protected override void SetShaderUniforms(CameraView camera)
    {
        Shader.SetVector3($"pointLights[{_index}].position", (Vector3)Transform.Position);
        Shader.SetVector3($"pointLights[{_index}].ambient", Ambient);
        Shader.SetVector3($"pointLights[{_index}].diffuse", Diffuse);
        Shader.SetVector3($"pointLights[{_index}].specular", Specular);
        Shader.SetFloat($"pointLights[{_index}].constant", Constant);
        Shader.SetFloat($"pointLights[{_index}].linear", Linear);
        Shader.SetFloat($"pointLights[{_index}].quadratic", Quadratic);

        base.SetShaderUniforms(camera);
    }

    /// <inheritdoc />
    public Task Render(CameraView camera, Window window)
    {
        SetShaderUniforms(camera);

        return Task.CompletedTask;
    }
}
