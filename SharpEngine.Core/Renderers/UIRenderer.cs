using SharpEngine.Core.Entities.UI;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using SharpEngine.Shared;
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
    private readonly CameraView _camera;
    private readonly Window _window;

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.UIRenderer;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    public UIRenderer(CameraView camera, Window window, ISettings settings, Scene scene) : base(settings)
    {
        _scene = scene;
        _uiShader = new UIShader();
        _camera = camera;
        _window = window;
    }

    /// <inheritdoc />
    public override Task Render()
    {
        if (_uiShader.Shader is null)
            return Task.CompletedTask;

        try
        {
            Window.GL.Enable(EnableCap.DepthTest);
            Window.GL.DepthFunc(DepthFunction.Less);

            // Disable face culling to render both sides of the quad
            Window.GL.Disable(EnableCap.CullFace);

            // _camera.SetShaderUniforms(_uiShader.Shader!);
            _uiShader.Shader?.Use();

            var uiElementRenderTasks = _scene.IterateAsync<UIElement>(_scene.UIElements, elem => elem.Render(_camera, _window));

            return Task.WhenAll(uiElementRenderTasks);
        }
        catch (Exception ex)
        {
            Debug.Log.Error(ex, "{Message}", ex.Message);
            return Task.FromException(ex);
        }
    }
}
