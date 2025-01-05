using Core.Interfaces;
using Core.Shaders;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Core.Renderers;

/// <summary>
///     Contains definitions shared across all renderers.
/// </summary>
public abstract class RendererBase
{
    private ISettings _settings;

    private List<Shader> _shaders = [];

    protected RendererBase(ISettings settings)
    {
        _settings = settings;
    }

    public abstract RenderFlags RenderFlag { get; }

    /// <summary>
    ///     Initializes the renderer.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    ///    Renders the scene.
    /// </summary>
    public void Render()
    {
        if (!_settings.RendererFlags.HasFlag(RenderFlag))
            return;

        // Set polygon mode to line to draw wireframe
        if (_settings.UseWireFrame)
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

        UseShaders();

        Render2();

        // Reset polygon mode to fill to draw solid objects
        if (!_settings.UseWireFrame)
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
    }

    private void UseShaders()
    {
        if (ShaderService.Instance.HasShadersToLoad)
            _shaders = ShaderService.Instance.GetAll();

        _shaders.ForEach(shader => shader.Use());
    }

    // TOOD: Rename properly
    public abstract void Render2();
}

[Flags]
public enum RenderFlags
{
    None = 0,
    Renderer3D = 1,
    UIRenderer = 2,
    All = 3
}
