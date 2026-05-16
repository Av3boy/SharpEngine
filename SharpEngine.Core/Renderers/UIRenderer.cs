using SharpEngine.Core.Entities.UI;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Windowing;
using Microsoft.Extensions.Logging;
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
    private readonly CameraView _camera;
    private readonly Window _window;
    private readonly ILogger<UIRenderer> _logger;

    private readonly GL _gl;

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.UIRenderer;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    public UIRenderer(CameraView camera, Window window, ISettings settings, Scene scene, ILogger<UIRenderer>? logger = null) : base(settings)
    {
        _scene = scene;
        _camera = camera;
        _window = window;
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UIRenderer>();

        _gl = window.GetGL();
    }

    /// <inheritdoc />
    public override Task Render()
    {
        // TODO: Make toggling these shaders into a hotkey.
        // return Task.CompletedTask;

        try
        {
            _gl.Enable(EnableCap.DepthTest);
            _gl.DepthFunc(DepthFunction.Less);

            // Disable face culling to render both sides of the quad
            _gl.Disable(EnableCap.CullFace);

            var uiElementRenderTasks = _scene.IterateAsync<UIElement>(_scene.UIElements, elem => elem.Render(_camera, _window));

            return Task.WhenAll(uiElementRenderTasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return Task.FromException(ex);
        }
    }
}
