using SharpEngine.Core.Handlers;
using SharpEngine.Core.Shaders;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SharpEngine.Core.EngineManagers;

public class ShaderManager : EngineHandler
{
    private List<Shader> _shaders = [];

    public ShaderManager(ILogger<ShaderManager> logger) : base(logger)
    {
    }

    /// <summary>
    ///     Uses all shaders in the shader service.
    /// </summary>
    /// <remarks>
    ///     This method should be called after the shaders have been loaded and compiled.
    ///     It will iterate through all shaders and call their Use() method to activate them for rendering.
    /// </remarks>
    public virtual void UseShaders()
    {
        if (ShaderService.Instance.HasShadersToLoad)
            _shaders = ShaderService.Instance.GetAll();

        if (_shaders is null)
            _shaders = new List<Shader>();

        foreach (var shader in _shaders)
        {
            try
            {
                shader.Use();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while using shader {Shader}.", shader.Name);
            }
        }
    }

    protected override Task ExecuteAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
