using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;
using System.Linq;

namespace Core.Renderers;

/// <summary>
///     Represents a renderer dedicated to drawing UI elements to the screen.
/// </summary>
public class UIRenderer : RendererBase
{
    private readonly Scene _scene;
    private readonly Camera _camera;

    private Shader? _uiShader;

    /// <summary>
    ///     Initializes a new instance of <see cref="UIRenderer"/>.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="camera"></param>
    public UIRenderer(Scene scene, Camera camera)
    {
        _scene = scene;
        _camera = camera;
    }

    private readonly float[] _vertices =
    {
        // Position         Texture coordinates
         0.5f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f,  // top right
         0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // top left
    };

    private readonly uint[] _indices =
    {
       0, 1, 3,
       1, 2, 3
    };

    private int _vertexArrayObject;

    /// <inheritdoc />
    public override void Initialize()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        InitializeBuffers();

        _uiShader = new Shader("Shaders/uiShader.vert", "Shaders/uiShader.frag", "ui");
        _uiShader.Use();

        InitializeVertexArrays();

        _scene.UIElements.Add(new UIElement()
        {
            Name = "uiElement",
        });
    }

    private void InitializeVertexArrays()
    {
        var vertexLocation = _uiShader!.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        var texCoordLocation = _uiShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
    }

    private void InitializeBuffers()
    {
        var vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        var elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
    }

    public void Render()
    {
        // TODO: New Enum flags to control the renderers that should draw to the screen (for debug purposes)
        // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(_vertexArrayObject);

        _uiShader!.Use();

        // TODO: Control ui element positions.
        var model = Matrix4.CreateTranslation(new Vector3(1, 1, 1)) * Matrix4.CreateRotationX(90) * Matrix4.CreateScale(1);
        // _uiShader.SetMatrix4("model", model);
        // _uiShader.SetMatrix4("view", _camera.GetViewMatrix());
        //_uiShader.SetMatrix4("uProjection", _orthoProjection, transpose: false);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void RenderScene(List<UIElement> elements)
    {
        foreach (var uiElement in elements)
        {
            /*var model = Matrix4.CreateTranslation(new Vector3(1, 1, 1)) * Matrix4.CreateRotationX(45) * Matrix4.CreateScale(1);
            _uiShader.SetMatrix4("model", model);
            _uiShader.SetMatrix4("view", _camera.GetViewMatrix());
            _uiShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);*/

            uiElement.Render(_uiShader!);

            var children = uiElement.Children.OfType<UIElement>().ToList();
            RenderScene(children);
        }
    }
}
