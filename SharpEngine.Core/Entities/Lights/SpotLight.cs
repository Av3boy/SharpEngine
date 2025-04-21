using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using System.Numerics;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.Lights;

/// <summary>
///     Represents a spotlight source.
/// </summary>
public class SpotLight : Light
{
    /// <summary>
    ///     Initializes a new instance of <see cref="SpotLight"/>.
    /// </summary>
    public SpotLight()
    {
        Direction = new Vector3(0.0f, -1.0f, 0.0f);
        CutOff = (float)System.Math.Cos(Math.DegreesToRadians(12.5f));
        OuterCutOff = (float)System.Math.Cos(Math.DegreesToRadians(15.0f));
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;

        Shader = ShaderService.Instance.LoadShader(Default.VertexShader, Default.FragmentShader, "lighting");
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

    protected override void SetShaderUniforms(CameraView camera)
    {
        Shader.SetVector3("spotLight.position", (Vector3)Transform.Position);
        Shader.SetVector3("spotLight.direction", Direction);
        Shader.SetVector3("spotLight.ambient", Ambient);
        Shader.SetVector3("spotLight.diffuse", Diffuse);
        Shader.SetVector3("spotLight.specular", Specular);
        Shader.SetFloat("spotLight.constant", Constant);
        Shader.SetFloat("spotLight.linear", Linear);
        Shader.SetFloat("spotLight.quadratic", Quadratic);
        Shader.SetFloat("spotLight.cutOff", CutOff);
        Shader.SetFloat("spotLight.outerCutOff", OuterCutOff);

        base.SetShaderUniforms(camera);
    }

    /// <inheritdoc />
    public override Task Render(CameraView camera, Window window)
    {
        SetShaderUniforms(camera);
    
        return Task.CompletedTask;
    }
}
