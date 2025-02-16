using Core.Interfaces;
using Core.Shaders;
using System;
using System.Collections.Generic;

namespace Core.Renderers;

/// <summary>
///     Contains definitions shared across all renderers.
/// </summary>
public abstract class RendererBase
{
    private ISettings _settings;


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
    public abstract void Render();
}

[Flags]
public enum RenderFlags
{
    None = 0,
    Renderer3D = 1,
    UIRenderer = 2,
    All = 3
}
