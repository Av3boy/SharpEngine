using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Xml.Linq;

namespace Core;

public class GameObject : SceneNode
{
    public GameObject() { }

    public GameObject(string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        Material.DiffuseMap = TextureService.Instance.LoadTexture(diffuseMapFile);
        Material.SpecularMap = TextureService.Instance.LoadTexture(specularMapFile);
        Material.Shader = ShaderService.Instance.LoadShader(vertShaderFile, fragShaderFile, "lighting");
    }

    // TODO: Cleanup these properties

    public Mesh Mesh { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; } = new(1, 1, 1);
    public Quaternion Quaternion { get; set; } = new();
    public Material Material { get; set; } = new();

    public virtual void Render(Camera camera, DirectionalLight directionalLight, PointLight[] pointLights, SpotLight spotLight)
    {
        // Render lights
        directionalLight.Render(Material.Shader);
        for (int i = 0; i < pointLights.Length; i++)
        {
            pointLights[i].Render(Material.Shader, i);
        }

        spotLight.Render(Material.Shader);

        Material.DiffuseMap.Use(TextureUnit.Texture0);
        Material.SpecularMap.Use(TextureUnit.Texture1);

        Material.Shader.SetInt("material.diffuse", Material.diffuseUnit);
        Material.Shader.SetInt("material.specular", Material.specularUnit);
        Material.Shader.SetVector3("material.specular", Material.Specular);
        Material.Shader.SetFloat("material.shininess", Material.Shininess);

        Matrix4 model = Matrix4.CreateTranslation(Position);
        model *= Matrix4.CreateFromAxisAngle(Quaternion.Axis, MathHelper.DegreesToRadians(Quaternion.Angle));
        Material.Shader.SetMatrix4("model", model);

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
