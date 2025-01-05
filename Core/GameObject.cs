using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Core;

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
    public Material Material { get; set; } = new();

    private Transform _transform = new();

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
    ///     Renders the game object using the specified camera.
    /// </summary>
    public virtual void Render()
    {
        Material.DiffuseMap.Use(TextureUnit.Texture0);
        Material.SpecularMap.Use(TextureUnit.Texture1);

        Material.Shader.SetInt("material.diffuse", Material.diffuseUnit);
        Material.Shader.SetInt("material.specular", Material.specularUnit);
        Material.Shader.SetVector3("material.specular", Material.Specular);
        Material.Shader.SetFloat("material.shininess", Material.Shininess);

        Material.Shader.SetMatrix4("model", Transform.ModelMatrix);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    /// <summary>
    ///     Gets the bounding box of the game object.
    /// </summary>
    public BoundingBox BoundingBox { get; set; }
}

public class Transform
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; } = new(1, 1, 1);
    public Quaternion Rotation { get; set; } = new();

    public Matrix4 ModelMatrix => Matrix4.CreateScale(Scale) * 
                                  Matrix4.CreateFromAxisAngle(Rotation.Axis, MathHelper.DegreesToRadians(Rotation.Angle)) *
                                  Matrix4.CreateTranslation(Position);
}