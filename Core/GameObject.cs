using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Xml.Linq;

namespace Core;

public class GameObject : SceneNode
{
    public GameObject() { }

    public GameObject(string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        DiffuseMap = TextureService.Instance.LoadTexture(diffuseMapFile);
        SpecularMap = TextureService.Instance.LoadTexture(specularMapFile);
        Shader = ShaderService.Instance.LoadShader(vertShaderFile, fragShaderFile);
    }

    public Mesh Mesh { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; } = new(1, 1, 1);
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }
    public Shader Shader { get; set; }
    public Quaternion Quaternion { get; set; } = new();
    public Material Material { get; set; } = new();

    public void Render(Camera camera, DirectionalLight DirectionalLight, PointLight[] PointLights, SpotLight SpotLight)
    {
        Shader.Use();

        Shader.SetMatrix4("view", camera.GetViewMatrix());
        Shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        Shader.SetVector3("viewPos", camera.Position);

        // Directional light
        Shader.SetVector3("dirLight.direction", DirectionalLight.Direction);
        Shader.SetVector3("dirLight.ambient", DirectionalLight.Ambient);
        Shader.SetVector3("dirLight.diffuse", DirectionalLight.Diffuse);
        Shader.SetVector3("dirLight.specular", DirectionalLight.Specular);

        // Point lights
        for (int i = 0; i < PointLights.Length; i++)
        {
            Shader.SetVector3($"pointLights[{i}].position", PointLights[i].Position);
            Shader.SetVector3($"pointLights[{i}].ambient", PointLights[i].Ambient);
            Shader.SetVector3($"pointLights[{i}].diffuse", PointLights[i].Diffuse);
            Shader.SetVector3($"pointLights[{i}].specular", PointLights[i].Specular);
            Shader.SetFloat($"pointLights[{i}].constant", PointLights[i].Constant);
            Shader.SetFloat($"pointLights[{i}].linear", PointLights[i].Linear);
            Shader.SetFloat($"pointLights[{i}].quadratic", PointLights[i].Quadratic);
        }

        // Spot light
        SpotLight.Position = camera.Position;
        SpotLight.Direction = camera.Front;

        Shader.SetVector3("spotLight.position", SpotLight.Position);
        Shader.SetVector3("spotLight.direction", SpotLight.Direction);
        Shader.SetVector3("spotLight.ambient", SpotLight.Ambient);
        Shader.SetVector3("spotLight.diffuse", SpotLight.Diffuse);
        Shader.SetVector3("spotLight.specular", SpotLight.Specular);
        Shader.SetFloat("spotLight.constant", SpotLight.Constant);
        Shader.SetFloat("spotLight.linear", SpotLight.Linear);
        Shader.SetFloat("spotLight.quadratic", SpotLight.Quadratic);
        Shader.SetFloat("spotLight.cutOff", SpotLight.CutOff);
        Shader.SetFloat("spotLight.outerCutOff", SpotLight.OuterCutOff);

        DiffuseMap.Use(TextureUnit.Texture0);
        SpecularMap.Use(TextureUnit.Texture1);

        Shader.SetInt("material.diffuse", Material.diffuseUnit);
        Shader.SetInt("material.specular", Material.specularUnit);
        Shader.SetVector3("material.specular", Material.Specular);
        Shader.SetFloat("material.shininess", Material.Shininess);

        Matrix4 model = Matrix4.CreateTranslation(Position);
        model *= Matrix4.CreateFromAxisAngle(Quaternion.Axis, MathHelper.DegreesToRadians(Quaternion.Angle));
        Shader.SetMatrix4("model", model);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public BoundingBox BoundingBox => CalculateBoundingBox();

    private BoundingBox CalculateBoundingBox()
    {
        Vector3 min = Position - (Scale / 2);
        Vector3 max = Position + (Scale / 2);
        return new BoundingBox(min, max);
    }
}

public class Quaternion
{
    public float Angle { get; set; }
    public Vector3 Axis { get; set; } = new(0, 1, 0);
}
