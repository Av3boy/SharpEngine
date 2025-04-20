using SharpEngine.Core._Resources;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using System.Numerics;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.Lights;

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

        Shader = ShaderService.Instance.LoadShader(Default.VertexShader, Default.FragmentShader, "lighting");
    }

    /// <inheritdoc />
    public Task Render(CameraView camera, Window window)
    {
        Shader.SetVector3("dirLight.direction", Direction);
        Shader.SetVector3("dirLight.ambient", Ambient);
        Shader.SetVector3("dirLight.diffuse", Diffuse);
        Shader.SetVector3("dirLight.specular", Specular);

        return Task.CompletedTask;
    }
}
