using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Rendering;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Textures;
using SharpEngine.Core.Windowing;
using SharpEngine.Core._Resources;
using SharpEngine.Telemetry;
using Vector2 = SharpEngine.Core.Numerics.Vector2;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

using Silk.NET.OpenGL;
using System.Numerics;
using System.Threading.Tasks;
using SharpEngine.Core.Components.Properties;

namespace SharpEngine.Core.Entities.UI;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : EmptyNode<Transform2D, Vector2>, IRenderable
{
    // TODO: #53 This should be coming from the entity component system
    public MeshRenderer MeshRenderer { get; set; }

    private readonly ShaderParameterBinder _paramBinder;

    /// <summary>Gets or sets the width of the ui element.</summary>
    [ShaderParameter("width", ShaderParameterType.Float)]
    public float Width { get; set; } = 10;

    /// <summary>Gets or sets the height of the ui element.</summary>
    [ShaderParameter("height", ShaderParameterType.Float)]
    public float Height { get; set; } = 10;

    [ShaderParameter("orthoMatrix", ShaderParameterType.Mat4)]
    private Matrix4x4 OrthoMatrix = Matrix4x4.Identity;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    public UIElement(GL gl) : this(gl, "UIElement") { }

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(GL gl, string name) : base(name)
    {
        // TODO: #5 Support custom meshes?
        var mesh = MeshService.Instance.LoadMesh(nameof(Primitives.Plane), Primitives.Plane.Mesh);
        var debugTexture = TextureService.Instance.LoadTexture(Default.DebugTexture);
        var material = new Material("defaultUiMaterial", debugTexture) { Shader = new UIShader(gl) };
        MeshRenderer = new MeshRenderer(mesh, material);

        _paramBinder = new ShaderParameterBinder(LoggingExtensions.CreateLogger<ShaderParameterBinder>());
        _paramBinder.Bind(this, material.Shader);
    }

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    /// <param name="camera">The camera view.</param>
    /// <param name="window">The window where the UI element is rendered.</param>
    public override Task Render(CameraView camera, Window window)
    {
        MeshRenderer.Mesh.Bind();
        MeshRenderer.Material.Shader.Use();
        MeshRenderer.Material.DiffuseMap!.Texture!.Use(TextureUnit.Texture0);

        OrthoMatrix = Matrix4x4.CreateOrthographicOffCenter(
            -window.Width / 2f,
            window.Width / 2f,
            -window.Height / 2f,
            window.Height / 2f,
            -1,
            1);

        _paramBinder.Apply(this, MeshRenderer.Material.Shader);
        MeshRenderer.Material.Shader.SetVector2("position", (System.Numerics.Vector2)Transform.Position);
        MeshRenderer.Material.Shader.SetFloat("rotation", Math.DegreesToRadians(Transform.Rotation.Angle));
        MeshRenderer.Material.Shader.SetInt("texture1", 0);
        MeshRenderer.Material.Shader.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);

        MeshRenderer.Mesh.Draw();

        return Task.CompletedTask;
    }
}
