using SharpEngine.Core.Attributes;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Textures;
using SharpEngine.Core.Windowing;

using Silk.NET.OpenGL;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a game object in the scene.
/// </summary>
public class GameObject : SceneNode
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/>.
    /// </summary>
    public GameObject()
    {
        // TODO: Using lightning shader here will most likely break stuff, it needs to be refactored out.
        var shader = ShaderService.Instance.LoadShader(_Resources.Default.VertexShader, _Resources.Default.FragmentShader, "lighting");
        var diffuse = TextureService.Instance.LoadTexture(_Resources.Default.DebugTexture);
        Material = new(shader, diffuse);
        
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/> with specified textures and shaders.
    /// </summary>
    /// <param name="diffuseMapFile">The file path of the diffuse map texture.</param>
    /// <param name="specularMapFile">The file path of the specular map texture.</param>
    /// <param name="vertShaderFile">The file path of the vertex shader.</param>
    /// <param name="fragShaderFile">The file path of the fragment shader.</param>
    public GameObject(string? diffuseMapFile = null, string? specularMapFile = null, string? vertShaderFile = null, string? fragShaderFile = null)
    {
        var shader = ShaderService.Instance.LoadShader(vertShaderFile ?? _Resources.Default.VertexShader, fragShaderFile ?? _Resources.Default.FragmentShader, "lighting");
        var diffuse = TextureService.Instance.LoadTexture(diffuseMapFile ?? _Resources.Default.DebugTexture);
        var specular = string.IsNullOrEmpty(specularMapFile) ? null : TextureService.Instance.LoadTexture(specularMapFile!);
        Material = new Material(shader, diffuse, specular);

        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
    }

    /// <summary>
    ///     Gets or sets the mesh of the game object.
    /// </summary>
    public List<Mesh> Mesh { get; set; } = [];

    /// <summary>
    ///     Gets or sets the material of the game object.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public Material Material { get; set; }

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
        Material.Shader.SetInt("material.diffuse", Material.diffuseUnit);

        if (Material.UseSpecularMap)
        {
            Material.SpecularMap.Use(TextureUnit.Texture1);
            Material.Shader.SetInt("material.specular", Material.specularUnit);
            Material.Shader.SetVector3("material.specular", Material.Specular);
            Material.Shader.SetFloat("material.shininess", Material.Shininess);
        }
        else
        {
            Material.Shader.SetInt("material.specular", 0);
            Material.Shader.SetVector3("material.specular", new System.Numerics.Vector3(0));
            Material.Shader.SetFloat("material.shininess", 0);
        }

            Material.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        Window.GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        return Task.CompletedTask;
    }
}
