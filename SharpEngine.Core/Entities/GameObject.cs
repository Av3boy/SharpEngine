using SharpEngine.Core.Attributes;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Entities.Interfaces;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.UI;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEngine.Core._Resources;
using SharpEngine.Core.Textures;
using EngineTexture = SharpEngine.Core.Components.Properties.Textures.Texture;
using Shader = SharpEngine.Core.Shaders.Shader;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a game object in the scene.
/// </summary>
public class GameObject : EmptyNode<Transform, Vector3>, IRenderable
{
    // TODO: Remove these two.
    protected record struct TempShaderDataContainer(string shaderVertPath, string shaderFragPath, string shaderName);
    protected readonly TempShaderDataContainer _tempShaderData;

    public List<IComponent> Components { get; } = new List<IComponent>();

    private MeshRenderer _renderer;

    /// <summary>
    ///     Gets or sets the shader of the game object.
    /// </summary>
    public Shader? Shader { get; set; }

    /// <summary>
    ///     Gets or sets the mesh of the game object.
    /// </summary>
    public Model? Model { get; set; }

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

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/>.
    /// </summary>
    public GameObject() : this(model: null!) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/> class with a model.
    /// </summary>
    /// <param name="model">The model.</param>
    public GameObject(Model model) : this(model, shader: null!)
    {
        // Shader = ShaderService.Instance.LoadShader(Window.SharedGL, _shaderVertPath, _shaderFragPath, _shaderName);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/> with specified textures and shaders.
    /// </summary>
    /// <param name="model">The model of the game object.</param>
    /// <param name="shader">The shader to be used by the game object.</param>
    public GameObject(Model model, Shader shader) : base(string.Empty)
    {
        Model = model;
        Shader = shader;

        var material = MaterialExtensions.Default(shader);
        _renderer = new MeshRenderer(model, material);
        Components.Add(_renderer);

        if (shader is not null)
            _tempShaderData = new TempShaderDataContainer(shader.VertPath, shader.FragPath, shader.Name);
        else
            _tempShaderData = new TempShaderDataContainer(_Resources.Default.VertexShader, _Resources.Default.FragmentShader, "lighting");
    }

    /// <summary>
    ///     Sets shader uniforms for model, view, and projection matrices.
    /// </summary>
    /// <param name="camera">The camera view used to retrieve view and projection matrices.</param>
    protected virtual void SetShaderUniforms(CameraView camera)
    {
        if (Shader is null)
            throw new NullReferenceException(nameof(Shader));

        Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);

        // TODO: One of these could be calculated once and "reloaded" if it changes.
        Shader.SetMatrix4(ShaderAttributes.View, camera.GetViewMatrix(), true);
        Shader.SetMatrix4(ShaderAttributes.Projection, camera.GetProjectionMatrix(), true);
    }

    /// <inheritdoc />
    public override Task Render(CameraView camera, Window window)
    {
        // TODO: This needs to removed later once fixed.
        if (Model is null || Model.Meshes is null || !Model.Meshes.Any())
            return Task.CompletedTask;

        if (Shader is null)
            Shader = ShaderService.Instance.LoadShader(window, _tempShaderData.shaderVertPath, _tempShaderData.shaderFragPath, _tempShaderData.shaderName);

        foreach (var mesh in Model.Meshes)
        {
            mesh.Bind();

            foreach (var texture in mesh.Textures)
                texture.Use();

            Shader.Use();
            SetShaderUniforms(camera);

            foreach (var material in mesh.Materials)
                material.SetUniformValues(Shader);

            mesh.Draw();
        }

        return Task.CompletedTask;
    }
}

public static class MaterialExtensions
{
    public static Material Default(Shader shader)
    {
        var debugTexture = TextureService.Instance.LoadTexture(SharpEngine.Core._Resources.Default.DebugTexture);
        var material = new Material("defaultMaterial", debugTexture) { Shader = shader };
        return material;
    }
}