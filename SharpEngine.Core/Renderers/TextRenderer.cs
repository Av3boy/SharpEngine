using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;

using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEngine.Telemetry;
using SharpEngine.Core.Entities;

namespace SharpEngine.Core.Renderers;

/// <summary>
///     A renderer responsible for rendering text in the scene.
/// </summary>
public class TextRenderer : RendererBase
{
    private readonly Scene _scene;
    private readonly CameraView _camera;
    private readonly ILogger<TextRenderer> _logger;

    private GL _gl = null!;

    // Caching to avoid expensive per-frame scene traversals
    private List<TextElement> _cachedTextElements = new();
    private HashSet<TextElement> _initializedElements = new();
    private volatile bool _needsRefresh = false;
    private int _frameCounter = 0;
    private const int RefreshIntervalFrames = 30; // refresh cache every N frames

    /// <inheritdoc />
    public override RenderFlags RenderFlag => RenderFlags.Text;

    /// <summary>
    ///     Initializes a new instance of <see cref="TextRenderer"/>.
    /// </summary>
    public TextRenderer(CameraView camera, ISettings settings, Scene scene)
        : this(camera, settings, scene, LoggingExtensions.CreateLogger<TextRenderer>()) { }

    // Handler reference for subscribing/unsubscribing to scene events
    private Action? _sceneChangedHandler;

    /// <summary>
    ///     Initializes a new instance of <see cref="TextRenderer"/>.
    /// </summary>
    public TextRenderer(CameraView camera, ISettings settings, Scene scene, ILogger<TextRenderer> logger) : base(settings)
    {
        _scene = scene;
        _camera = camera;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override void OnWindowAttached(Window window)
    {
        _gl = window.GetGL();

        // Prime cache immediately so first frames don't pay traversal cost
        RefreshTextCache();
        _logger.LogTrace("TextRenderer.OnWindowAttached: cache primed, cachedCount={Count}", _cachedTextElements.Count);

        // Initialize cached elements
        foreach (var node in _cachedTextElements)
        {
            try
            {
                node.OnInitialized(_gl);
                _initializedElements.Add(node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing text element: {Message}", ex.Message);
            }
        }

        // Subscribe to scene changes to refresh cache on demand. Only mark that a refresh is needed to avoid doing
        // heavy traversals for every scene mutation; the renderer will perform a single refresh per frame when needed.
        _sceneChangedHandler = () => _needsRefresh = true;
        _scene.SceneChanged += _sceneChangedHandler;
    }

    private void RefreshTextCache()
    {
        // Discover Text elements from the scene graph root so examples that attach text under Root are found.
        var newList = _scene.Root.GetObjectsOfType<TextElement>().ToList();

        // Determine which elements are newly added and which were removed
        var newSet = new HashSet<TextElement>(newList);
        var added = newList.Where(e => !_initializedElements.Contains(e)).ToList();
        var removed = _initializedElements.Where(e => !newSet.Contains(e)).ToList();

        // Update internal caches
        _cachedTextElements = newList;

        // Initialize newly added elements
        foreach (var node in added)
        {
            try
            {
                node.OnInitialized(_gl);
                _initializedElements.Add(node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing newly added Text element: {Message}", ex.Message);
            }
        }

        // Remove references to removed elements
        foreach (var node in removed)
            _initializedElements.Remove(node);

        try
        {
            _logger.LogTrace("RefreshTextCache: found {Count} text elements, root children={RootChildren}", _cachedTextElements.Count, _scene.Root.Children.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing text cache: {Message}", ex.Message);
        }
    }

    /// <inheritdoc />
    public override async Task Render()
    {
        try
        {
            // If scene changed since last frame, refresh cache once per frame to coalesce bursty updates.
            if (_needsRefresh)
            {
                _needsRefresh = false;
                RefreshTextCache();
            }

            // Use the cached text elements. Cache is maintained in OnWindowAttached and via the Scene.SceneChanged event.
            _gl.Enable(EnableCap.DepthTest);
            _gl.DepthFunc(DepthFunction.Less);

            // Disable face culling to render both sides of glyph meshes
            _gl.Disable(EnableCap.CullFace);

            _frameCounter++;
            var swTotal = System.Diagnostics.Stopwatch.StartNew();
            var swRender = System.Diagnostics.Stopwatch.StartNew();

            // Render each text element sequentially on the GL thread. Avoid per-element Task allocations for performance.
            foreach (var elem in _cachedTextElements)
            {
                await elem.Render(_camera, Window);
            }

            swRender.Stop();
            swTotal.Stop();

            // Log sampling: every RefreshIntervalFrames frames log timings to help profiling
            if (_frameCounter % RefreshIntervalFrames == 0)
                _logger.LogTrace("Text render: total={TotalMs}ms, draw={DrawMs}ms, elements={Count}", swTotal.Elapsed.TotalMilliseconds, swRender.Elapsed.TotalMilliseconds, _cachedTextElements.Count);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                if (_sceneChangedHandler != null)
                    _scene.SceneChanged -= _sceneChangedHandler;
            }

            base.Dispose(disposing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while disposing TextRenderer: {Message}", ex.Message);
        }
    }
}
