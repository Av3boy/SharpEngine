using Core.Entities;
using Core.Interfaces;
using Core.Shaders;
using Silk.NET.OpenGL;
using System;
using System.Threading.Tasks;

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
        if (_uiShader.Shader is null)
        {
            Console.WriteLine("The UI shader is null. This is broken mostly when the exe is started outside the solution. This needs to be fixed later.");
            return;
        }   
        
        _uiShader.Shader.Use();

        _scene.UIElements.Add(new UIElement("uiElement"));

        _scene.Iterate(_scene.UIElements, elem => elem.Initialize());

        InitializeVertexArrays();
    }

    private void InitializeVertexArrays()
    {
        _uiShader.SetAttributes();
    }

    /// <inheritdoc />
    public override async Task Render()
    {
        Window.GL.Disable(EnableCap.DepthTest);
        Window.GL.DepthFunc(DepthFunction.Less);

        _uiShader.Shader.Use();
        await _scene.IterateAsync(_scene.UIElements, elem => elem.Render());
    }
}
