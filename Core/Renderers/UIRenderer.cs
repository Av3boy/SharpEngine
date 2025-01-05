using Core.Entities;
using Core.Interfaces;
using Core.Shaders;

using OpenTK.Graphics.OpenGL4;

namespace Core.Renderers;

/// <summary>
///     Represents a renderer dedicated to drawing UI elements to the screen.
/// </summary>
public class UIRenderer : RendererBase
{
    private readonly Scene _scene;

    private readonly UIShader _uiShader;

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.UIRenderer;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="game"></param>
    public UIRenderer(Scene scene, IGame game) : base(game.CoreSettings)
    {
        _scene = scene;

        _uiShader = new UIShader();
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        _uiShader.Shader.Use();

        _scene.UIElements.Add(new UIElement()
        {
            Name = "uiElement",
        });

        _scene.Iterate(_scene.UIElements, elem => elem.Initialize());

        InitializeVertexArrays();
    }

    private void InitializeVertexArrays()
    {
        _uiShader.SetAttributes();
    }

    /// <inheritdoc />
    public override void Render2()
    {
        GL.Disable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);

        _uiShader.Shader.Use();
        _scene.Iterate(_scene.UIElements, elem => elem.Render());
    }
}
