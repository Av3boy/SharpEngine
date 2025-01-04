using Core.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Core.Renderers;

/// <summary>
///     Represents a renderer dedicated to drawing UI elements to the screen.
/// </summary>
public class UIRenderer : RendererBase
{
    private readonly Scene _scene;
    private readonly Camera _camera;

    private readonly UIShader _uiShader;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="camera"></param>
    public UIRenderer(Scene scene, Camera camera)
    {
        _scene = scene;
        _camera = camera;

        _uiShader = new UIShader();
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

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

    public override void Render()
    {
        // TODO: New Enum flags to control the renderers that should draw to the screen (for debug purposes)
        // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _uiShader.Shader.Use();

        _scene.Iterate(_scene.UIElements, elem => elem.Render());
    }
}
