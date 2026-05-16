using SharpEngine.Core.Attributes;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Components.Properties;
using EngineTexture = SharpEngine.Core.Components.Properties.Textures.Texture;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using Shader = SharpEngine.Core.Shaders.Shader;

using Silk.NET.OpenGL;
using Tutorial;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a game object in the scene.
/// </summary>
public class GameObject : EmptyNode<Transform, Vector3>, IRenderable
{
    private readonly object _modelCacheLock = new();
    private readonly Dictionary<object, Model> _modelByShareGroup = [];

    private readonly string _shaderVertPath;
    private readonly string _shaderFragPath;
    private readonly string _shaderName;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/>.
    /// </summary>
    public GameObject() : base(string.Empty)
    {
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);

        _shaderVertPath = _Resources.Default.VertexShader;
        _shaderFragPath = _Resources.Default.FragmentShader;
        _shaderName = "lighting";
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/> class with a model.
    /// </summary>
    /// <param name="model">The model.</param>
    public GameObject(Model model) : base(string.Empty)
    {
        Model = model;
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);

        _shaderVertPath = _Resources.Default.VertexShader;
        _shaderFragPath = _Resources.Default.FragmentShader;
        _shaderName = "lighting";
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject"/> with specified textures and shaders.
    /// </summary>
    /// <param name="shader">The shader to be used by the game object.</param>
    /// <param name="model">The model of the game object.</param>
    public GameObject(Shader shader, Model model) : base(string.Empty)
    {
        Model = model;
        BoundingBox = BoundingBox.CalculateBoundingBox(Transform);
        Shader = shader;

        _shaderVertPath = shader.VertPath;
        _shaderFragPath = shader.FragPath;
        _shaderName = shader.Name;
    }

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
        if (Model is null || Model.Meshes is null)
            return Task.CompletedTask;

        var glInstance = window.GetGL();

        // Ensure this instance is using a shader compiled for the correct context/share-group.
        Shader = ShaderService.Instance.LoadShader(window, _shaderVertPath, _shaderFragPath, _shaderName);

        var modelToRender = GetOrCreateModelForWindow(window, glInstance);
        if (modelToRender is null)
            return Task.CompletedTask;

        foreach (var mesh in modelToRender.Meshes)
        {
            mesh.Bind();

            foreach (var texture in mesh.Textures)
                texture.Use();

            Shader.Use();
            SetShaderUniforms(camera);

            foreach (var material in mesh.Materials)
                material.SetUniformValues(Shader);

            if (mesh.Indices.Length > 0)
                glInstance.DrawElements<uint>(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, []);
            else
                glInstance.DrawArrays(PrimitiveType.Triangles, 0, (uint)(mesh.Vertices.Length / (VertexData.VerticesSize + VertexData.NormalsSize + VertexData.TexCoordsSize)));
        }

        return Task.CompletedTask;
    }

    private static object GetShareGroupKey(Window window)
        => (object?)window.SharedContext ?? (object?)window.GLContext ?? window;

    private Model? GetOrCreateModelForWindow(Window window, GL gl)
    {
        if (Model is null)
            return null;

        var shareGroupKey = GetShareGroupKey(window);

        lock (_modelCacheLock)
        {
            if (_modelByShareGroup.TryGetValue(shareGroupKey, out var cachedModel))
                return cachedModel;

            // If this model was created using the same GL instance, we can reuse it for this share group.
            // Otherwise, build a copy of the model bound to the GL instance for this window.
            var canReuseModel = Model.Meshes.Count > 0 && ReferenceEquals(Model.Meshes[0].GL, gl);

            var modelForWindow = canReuseModel ? Model : CloneModel(gl, Model);
            _modelByShareGroup[shareGroupKey] = modelForWindow;

            return modelForWindow;
        }
    }

    private static Model CloneModel(GL gl, Model template)
    {
        // Create an empty model and populate it with meshes recreated against the provided GL instance.
        // This allows rendering on windows that don't share the original context.
        var clone = new Model(gl, string.Empty);

        foreach (var mesh in template.Meshes)
            clone.Meshes.Add(CloneMesh(gl, mesh));

        return clone;
    }

    private static Mesh CloneMesh(GL gl, Mesh template)
    {
        var textures = template.Textures is null ? 
            new List<EngineTexture>() :
            [.. template.Textures.Select(t => new EngineTexture(gl, t.Path, t.Type))];

        var clone = new Mesh(gl, template.Vertices, template.Indices, textures)
        {
            Name = template.Name,
        };

        if (template.Materials is { Count: > 0 })
            clone.Materials = [.. template.Materials.Select(m => CloneMaterial(gl, m))];

        return clone;
    }

    private static Material CloneMaterial(GL gl, Material template)
    {
        var diffuse = new EngineTexture(gl, template.DiffuseMap.Path, template.DiffuseMap.Type);

        EngineTexture? specular = null;
        if (template.UseSpecularMap)
            specular = new EngineTexture(gl, template.SpecularMap.Path, template.SpecularMap.Type);

        var clone = new Material(diffuse, specular)
        {
            Name = template.Name,
            DiffuseTextureMap = template.DiffuseTextureMap,
            SpecularTextureMap = template.SpecularTextureMap,
            Specular = template.Specular,
            Shininess = template.Shininess,
            AmbientColor = template.AmbientColor,
            DiffuseColor = template.DiffuseColor,
            SpecularColor = template.SpecularColor,
            SpecularCoefficient = template.SpecularCoefficient,
            Transparency = template.Transparency,
            IlluminationModel = template.IlluminationModel,
            AmbientTextureMap = template.AmbientTextureMap,
            SpecularHighlightTextureMap = template.SpecularHighlightTextureMap,
            BumpMap = template.BumpMap,
            DisplacementMap = template.DisplacementMap,
            StencilDecalMap = template.StencilDecalMap,
            AlphaTextureMap = template.AlphaTextureMap,
        };

        return clone;
    }
}
