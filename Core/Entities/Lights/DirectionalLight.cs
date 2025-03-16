using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Shaders;
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

        Material.Shader = ShaderService.Instance.LoadShader(PathExtensions.GetAssemblyPath("Shaders/shader.vert"), PathExtensions.GetAssemblyPath("Shaders/lighting.frag"), "lighting");
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
