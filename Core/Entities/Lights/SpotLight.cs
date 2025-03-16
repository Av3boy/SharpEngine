using SharpEngine.Core.Extensions;
using SharpEngine.Core.Shaders;
using System.Numerics;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.Lights;

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

        Material.Shader = ShaderService.Instance.LoadShader(PathExtensions.GetAssemblyPath("Shaders/shader.vert"), PathExtensions.GetAssemblyPath("Shaders/lighting.frag"), "lighting");
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
