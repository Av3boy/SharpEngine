using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Textures;
using SharpEngine.Core.Windowing;
using SharpEngine.Core._Resources;
using Vector2 = SharpEngine.Core.Numerics.Vector2;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

using Silk.NET.OpenGL;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.UI;

/// <summary>
///     Represents a User Interface entity.
/// </summary>
public class UIElement : EmptyNode<Transform2D, Vector2>, IRenderable
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    public UIElement() : this("UIElement") { }

    /// <summary>
    ///     Initializes a new instance of <see cref="UIElement"/>.
    /// </summary>
    /// <param name="name">The name of the UI element.</param>
    public UIElement(string name) : base(name)
    {
        // TODO: #5 Support custom meshes?
        Mesh = MeshService.Instance.LoadMesh(nameof(Primitives.Plane), Primitives.Plane.Mesh);
    }

    private readonly UIShader _uiShader = new();
    private readonly Texture _texture = TextureService.Instance.LoadTexture(Default.DebugTexture);

    /// <summary>Gets or sets the width of the ui element.</summary>
    public float Width { get; set; } = 10;

    /// <summary>Gets or sets the height of the ui element.</summary>
    public float Height { get; set; } = 10;

    /// <summary>Gets or sets the mesh of the UI element.</summary>
    public Mesh Mesh { get; set; }

    /// <summary>
    ///     Gets the most recently used VAO for this element.
    /// </summary>
    /// <remarks>
    ///     UIElement maintains VAOs per OpenGL context. This property is updated on render.
    /// </remarks>
    public uint VAO { get; private set; }

    private sealed record SharedBuffers(uint Vbo, uint Ebo, int IndexCount);
    private sealed record ContextState(uint Vao);

    private readonly object _gpuLock = new();
    private readonly Dictionary<object, SharedBuffers> _sharedBuffersByShareGroup = [];
    private readonly Dictionary<object, ContextState> _contextStateByContext = [];

    private static object GetShareGroupKey(Window window)
        => (object?)window.SharedContext ?? (object?)window.GLContext ?? (object)window;

    private static object GetContextKey(Window window)
        => (object?)window.GLContext ?? (object)window;

    private SharedBuffers EnsureSharedBuffers(GL gl, Window window)
    {
        var shareGroupKey = GetShareGroupKey(window);

        lock (_gpuLock)
        {
            if (_sharedBuffersByShareGroup.TryGetValue(shareGroupKey, out var buffers))
                return buffers;

            // Buffers are shareable across contexts in the same share group.
            var vbo = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
            gl.BufferData(GLEnum.ArrayBuffer, Mesh.GetVertices(), GLEnum.StaticDraw);

            var ebo = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ElementArrayBuffer, ebo);
            gl.BufferData(GLEnum.ElementArrayBuffer, Mesh.Indices, GLEnum.StaticDraw);

            buffers = new SharedBuffers(vbo, ebo, Mesh.Indices.Length);
            _sharedBuffersByShareGroup[shareGroupKey] = buffers;

            return buffers;
        }
    }

    private ContextState EnsureContextState(GL gl, Window window, SharedBuffers sharedBuffers)
    {
        var contextKey = GetContextKey(window);

        lock (_gpuLock)
        {
            if (_contextStateByContext.TryGetValue(contextKey, out var state))
                return state;

            // VAOs are generally not shared between contexts.
            var vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            gl.BindBuffer(GLEnum.ArrayBuffer, sharedBuffers.Vbo);
            gl.BindBuffer(GLEnum.ElementArrayBuffer, sharedBuffers.Ebo);

            _uiShader.EnsureInitialized(window);
            _uiShader.Shader!.Use();
            _uiShader.SetAttributes(gl);

            state = new ContextState(vao);
            _contextStateByContext[contextKey] = state;

            return state;
        }
    }

    Matrix4x4 OrthoMatrix = Matrix4x4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public override Task Render(CameraView camera, Window window)
    {
        var gl = window.GetGL();

        _uiShader.EnsureInitialized(window);

        var sharedBuffers = EnsureSharedBuffers(gl, window);
        var contextState = EnsureContextState(gl, window, sharedBuffers);

        _uiShader.Shader!.Use();
        gl.BindVertexArray(contextState.Vao);
        _texture.Use(TextureUnit.Texture0);

        VAO = contextState.Vao;

        // TODO: #75 These should come from somewhere else.
        const float screenWidth = 1280;
        const float screenHeight = 720;

        _uiShader.Shader!.SetFloat("width", Width);
        _uiShader.Shader!.SetFloat("height", Height);
        _uiShader.Shader!.SetVector2("screenSize", new System.Numerics.Vector2(screenWidth, screenHeight));
        _uiShader.Shader!.SetVector2("position", (System.Numerics.Vector2)Transform.Position);
        _uiShader.Shader!.SetFloat("rotation", Math.DegreesToRadians(Transform.Rotation.Angle));
        _uiShader.Shader!.SetInt("texture1", 0);
        _uiShader.Shader!.SetMatrix4(ShaderAttributes.Model, Transform.ModelMatrix);
        _uiShader.Shader!.SetMatrix4("orthoMatrix", OrthoMatrix); // Pass the orthographic matrix to the shader

        gl.DrawElements<uint>(PrimitiveType.Triangles, (uint)sharedBuffers.IndexCount, DrawElementsType.UnsignedInt, []);

        return Task.CompletedTask;
    }
}
