using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;

using System;
using System.Threading.Tasks;

namespace SharpEngine.Core.Renderers;

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
    public UIRenderer(CameraView _, ISettings settings, Scene scene) : base(settings)
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
    public override Task Render()
    {
        try
        {
            Window.GL.Disable(EnableCap.DepthTest);
            Window.GL.DepthFunc(DepthFunction.Less);

            foreach (var item in _scene.UIElements)
            {
                item.Bind();
            }

            _uiShader.Shader.Use();

            var uiElementRenderTasks = _scene.IterateAsync<UIElement>(_scene.UIElements, elem => elem.Render());

            return Task.WhenAll(uiElementRenderTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Task.FromException(ex);
        }
    }
}
