using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Handlers;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEngine.Core.Managers;

public class RendererManager : EngineHandler
{
    private readonly RendererBase[] _registeredRenderers;
    private ISettings _settings;

    public RendererManager(ISettings settings, IEnumerable<RendererBase>? renderers = null) : base(null)
    {
        _settings = settings;
        _registeredRenderers = renderers?.ToArray() ?? [];
    }

    public void Run()
    {
        var activeRenderers = _registeredRenderers.Where(renderer => _settings.RendererFlags.HasFlag(renderer.RenderFlag))
            .OrderBy(renderer => renderer.RenderFlag)
            .ToList();

        foreach (var renderer in activeRenderers)
            renderer.Render().GetAwaiter().GetResult();
    }

    public RendererBase[] CreateRenderers(CameraView camera, Scene scene)
    {
        // Return the renderers that have been registered
        if (_registeredRenderers.Length > 0)
            return _registeredRenderers;

        // When no renderers have been registered, fall back to use all that are discoverable.
        var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(RendererBase)) && !type.IsAbstract);

        return [.. rendererTypes
            .Select(type =>
            {
                var requiredArguments = new object[] { camera, _settings, scene };
                return (RendererBase)Activator.CreateInstance(type, requiredArguments)!;
            })];
    }

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
