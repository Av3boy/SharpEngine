using Core.Renderers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Xml.Linq;

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
    }

    // TODO: Cleanup these properties

    /// <summary>
    ///     Gets or sets the mesh of the game object.
    /// </summary>
    public Mesh Mesh { get; set; }

    /// <summary>
    ///     Gets or sets the position of the game object.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    ///     Gets or sets the scale of the game object.
    /// </summary>
    public Vector3 Scale { get; set; } = new(1, 1, 1);

    /// <summary>
    ///     Gets or sets the rotation of the game object.
    /// </summary>
    public Quaternion Quaternion { get; set; } = new();

    /// <summary>
    ///     Gets or sets the material of the game object.
    /// </summary>
    public Material Material { get; set; } = new();

    /// <summary>
    ///     Renders the game object using the specified camera.
    /// </summary>
    /// <param name="camera">The camera to use for rendering.</param>
    public virtual void Render(Camera camera)
    {
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

    // TODO: Calculate during initialization. Should primitive types contain hard coded values?
    /// <summary>
    ///     Gets the bounding box of the game object.
    /// </summary>
    public BoundingBox BoundingBox => CalculateBoundingBox();

    /// <summary>
    ///     Calculates the bounding box of the game object.
    /// </summary>
    /// <returns>The bounding box of the game object.</returns>
    private BoundingBox CalculateBoundingBox()
    {
        Vector3 min = Position - (Scale / 2);
        Vector3 max = Position + (Scale / 2);
        return new BoundingBox(min, max);
    }
}

/// <summary>
///     Represents a quaternion for rotation.
/// </summary>
public class Quaternion
{
    /// <summary>
    ///     Gets or sets the angle of rotation in degrees.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    ///     Gets or sets the axis of rotation.
    /// </summary>
    public Vector3 Axis { get; set; } = new(0, 1, 0);
}
