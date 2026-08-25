using System.Collections.Generic;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Rendering;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using Vector2 = SharpEngine.Core.Numerics.Vector2;

using Silk.NET.OpenGL;
using System.Numerics;
using System.Threading.Tasks;
using SharpEngine.Core.Entities.Interfaces;

namespace SharpEngine.Core.Entities.UI;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : EmptyNode<Transform2D, Vector2>, IRenderable
{
    public List<IComponent> Components { get; } = [];
    private MeshRenderer _renderer;
    private ShaderParameterBinder _paramBinder;

    /// <summary>Gets or sets the width of the ui element.</summary>
    [ShaderParameter("width", ShaderParameterType.Float)]
    public float Width { get; set; } = 10;

    /// <summary>Gets or sets the height of the ui element.</summary>
    [ShaderParameter("height", ShaderParameterType.Float)]
    public float Height { get; set; } = 10;

    [ShaderParameter("orthoMatrix", ShaderParameterType.Mat4)]
    public Matrix4x4 OrthoMatrix = Matrix4x4.Identity;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    public UIElement() : this("UIElement") { }

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(string name) : base(name) { }

    /// <inheritdoc />
    public override void OnInitialized(GL gl)
    {
        base.OnInitialized(gl);

        // TODO: #5 Support custom meshes?
        var mesh = MeshService.Instance.LoadMesh(nameof(Primitives.Plane), Primitives.Plane.Mesh);
        var material = MaterialExtensions.Default(new UIShader(gl));

        _renderer = new MeshRenderer(mesh, material);
        Components.Add(_renderer);

        _paramBinder = new ShaderParameterBinder();
        _paramBinder.Bind(this, material.Shader);
    }

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    /// <param name="camera">The camera view.</param>
    /// <param name="window">The window where the UI element is rendered.</param>
    public override Task Render(CameraView camera, Window window)
    {
        _renderer.Mesh.Bind();
        _renderer.Material.Shader.Use();
        _renderer.Material.DiffuseMap!.Texture!.Use(TextureUnit.Texture0);

        OrthoMatrix = window.CreateOrthographicOffCenter();

        _paramBinder.Apply(this, _renderer.Material.Shader);
        _renderer.Material.Shader.SetVector2("position", (System.Numerics.Vector2)Transform.Position);
        _renderer.Material.Shader.SetFloat("rotation", Math.DegreesToRadians(Transform.Rotation.Angle));
        _renderer.Material.Shader.SetInt("texture1", 0);
        _renderer.Material.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);

        _renderer.Mesh.Draw();

        return Task.CompletedTask;
    }
}
