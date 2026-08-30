using SharpEngine.Core.Handlers;
using SharpEngine.Core.Shaders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SharpEngine.Core.Managers;

public class ShaderManager : EngineHandler
{
    private List<Shader> _shaders = [];

    public ShaderManager(ILogger<ShaderManager> logger) : base(logger)
    {
    }

    public virtual void UseShaders()
    {
        if (ShaderService.Instance.HasShadersToLoad)
            _shaders = ShaderService.Instance.GetAll();

        _shaders.ForEach(shader => shader.Use());
    }

    protected override Task ExecuteAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
