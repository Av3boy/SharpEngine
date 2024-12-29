using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Core.Renderers;
public class UIRenderer : IRenderer
{
    private readonly Scene _scene;
    private readonly Camera _camera;

    private Shader _uiShader;

    private int _vaoModel;

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

    private int _elementBufferObject;

    private int _vertexBufferObject;

    private int _vertexArrayObject;

    public void Initialize()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        _uiShader = new Shader("Shaders/uiShader.vert", "Shaders/uiShader.frag", "ui");
        _uiShader.Use();

        var vertexLocation = _uiShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        var texCoordLocation = _uiShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

        _scene.UIElements.Add(new UIElement()
        {
            Name = "uiElement",
        });
    }

    private void InitializeVertexArrays()
    {
        _vaoModel = GL.GenVertexArray();
        GL.BindVertexArray(_vaoModel);

        var positionLocation = _uiShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

        var normalLocation = _uiShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

        var texCoordLocation = _uiShader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);
    }

    private void InitializeBuffers()
    {
        var vertexBufferObject = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
    }

    public void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BindVertexArray(_vertexArrayObject);

        _uiShader.Use();

        var model = Matrix4.CreateTranslation(new Vector3(1, 1, 1)) * Matrix4.CreateRotationX(90) * Matrix4.CreateScale(1);
        _uiShader.SetMatrix4("model", model);
        _uiShader.SetMatrix4("view", _camera.GetViewMatrix());
        _uiShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        // GL.DrawArrays(PrimitiveType.Triangles, 0, 2);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void RenderScene(List<UIElement> elements)
    {
        foreach (var uiElement in elements)
        {
            var model = Matrix4.CreateTranslation(new Vector3(1, 1, 1)) * Matrix4.CreateRotationX(45) * Matrix4.CreateScale(1);
            _uiShader.SetMatrix4("model", model);
            _uiShader.SetMatrix4("view", _camera.GetViewMatrix());
            _uiShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            //uiElement.Render(_uiShader);

            var children = uiElement.Children.OfType<UIElement>().ToList();
            RenderScene(children);
        }
    }
}
