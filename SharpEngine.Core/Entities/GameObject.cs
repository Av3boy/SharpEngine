using SharpEngine.Core.Attributes;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using Shader = SharpEngine.Core.Shaders.Shader;

using Silk.NET.OpenGL;
using Tutorial;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a game object in the scene.
/// </summary>
public class GameObject : EmptyNode<Transform, Vector3>, IRenderable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/>.
    /// </summary>
    public GameObject() : base(string.Empty)
    {
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
        Shader = ShaderService.Instance.LoadShader(_Resources.Default.VertexShader, _Resources.Default.FragmentShader, "lighting");
    }

    public GameObject(Model_Old model) : base(string.Empty)
    {
        Model = model;
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
        Shader = ShaderService.Instance.LoadShader(_Resources.Default.VertexShader, _Resources.Default.FragmentShader, "lighting");
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/> with specified textures and shaders.
    /// </summary>
    /// <param name="diffuseMapFile">The file path of the diffuse map texture.</param>
    /// <param name="specularMapFile">The file path of the specular map texture.</param>
    /// <param name="vertShaderFile">The file path of the vertex shader.</param>
    /// <param name="fragShaderFile">The file path of the fragment shader.</param>
    public GameObject(Shader shader, Model_Old model) : base(string.Empty)
    {
        Model = model;
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
        Shader = shader;
    }

    public Shader Shader { get; set; }

    /// <summary>
    ///     Gets or sets the mesh of the game object.
    /// </summary>
    public Model_Old Model { get; set; }

    /// <summary>
    ///    Gets or sets the transform of the game object.
    /// </summary>
    public override Transform Transform 
    {
        get => _transform;
        set
        {
            _transform = value;
            BoundingBox = BoundingBox.CalculateBoundingBox(_transform);
        }
    }
    
    private Transform _transform = new();

    /// <summary>
    ///     Gets the bounding box of the game object.
    /// </summary>
    [Inspector(DisplayInInspector = false)]
    public BoundingBox BoundingBox { get; set; }

    protected virtual void SetShaderUniforms(CameraView camera)
    {
        Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        Shader.SetMatrix4("uView", camera.GetViewMatrix(), false);
        Shader.SetMatrix4("uProjection", camera.GetProjectionMatrix(), false);
    }

    /// <inheritdoc />
    public Task Render(CameraView camera, Window window)
    {
        // TODO: This needs to removed later once fixed.
        if (Model is null || Model.Meshes is null)
            return Task.CompletedTask;

        foreach (var mesh in Model.Meshes)
        {
            mesh.Bind();

            foreach (var texture in mesh.Textures)
                texture.Use();

            Shader.Use();
            SetShaderUniforms(camera);

            foreach (var material in mesh.Materials)
                material.SetUniformValues(Shader);

            Window.GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Vertices.Length);
        }

        return Task.CompletedTask;
    }
}
