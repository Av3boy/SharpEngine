using SharpEngine.Core.Attributes;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Textures;
using Silk.NET.OpenGL;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities;

/// <summary>
/// Represents a game object in the scene.
/// </summary>
public class GameObject : SceneNode
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/>.
    /// </summary>
    public GameObject() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameObject"/> with specified textures and shaders.
    /// </summary>
    /// <param name="diffuseMapFile">The file path of the diffuse map texture.</param>
    /// <param name="specularMapFile">The file path of the specular map texture.</param>
    /// <param name="vertShaderFile">The file path of the vertex shader.</param>
    /// <param name="fragShaderFile">The file path of the fragment shader.</param>
    public GameObject(string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        Material.DiffuseMap = TextureService.Instance.LoadTexture(diffuseMapFile);
        Material.SpecularMap = TextureService.Instance.LoadTexture(specularMapFile);
        Material.Shader = ShaderService.Instance.LoadShader(vertShaderFile, fragShaderFile, "lighting");

        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
    }

    // TODO: Cleanup these properties

    /// <summary>
    ///     Gets or sets the mesh of the game object.
    /// </summary>
    public Mesh Mesh { get; set; }

    /// <summary>
    ///     Gets or sets the material of the game object.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public Material Material { get; set; } = new();

    private Transform _transform = new();

    /// <summary>
    ///    Gets or sets the transform of the game object.
    /// </summary>
    public Transform Transform
    {
        get => _transform;
        set
        {
            _transform = value;
            BoundingBox = BoundingBox.CalculateBoundingBox(_transform);
        }
    }

    /// <summary>
    ///     Gets the bounding box of the game object.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public BoundingBox BoundingBox { get; set; }

    /// <inheritdoc />
    public override Task Render()
    {
        Material.DiffuseMap.Use(TextureUnit.Texture0);
        Material.SpecularMap.Use(TextureUnit.Texture1);

        Material.Shader.SetInt("material.diffuse", Material.diffuseUnit);
        Material.Shader.SetInt("material.specular", Material.specularUnit);
        Material.Shader.SetVector3("material.specular", Material.Specular);
        Material.Shader.SetFloat("material.shininess", Material.Shininess);

        Material.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);

        Window.GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        return Task.CompletedTask;
    }
}
