using Microsoft.Extensions.DependencyInjection;
using SharpEngine.Core.Handlers;
using System;
using System.Collections.Generic;

namespace SharpEngine.Core.DependencyInjection;

public interface IHandlerRegistration
{
    void Apply(IServiceProvider serviceProvider, Engine engine);
}

public sealed class HandlerRegistrationBuilder<THandler> : IHandlerRegistration where THandler : class
{
    private readonly List<Action<IServiceProvider, THandler, Engine>> _configurations = [];

    public HandlerRegistrationBuilder<THandler> Configure(Action<IServiceProvider, THandler, Engine> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        _configurations.Add(configure);
        return this;
    }

    public void Apply(IServiceProvider serviceProvider, Engine engine)
    {
        var handler = serviceProvider.GetRequiredService<THandler>();

        if (handler is not EngineHandler engineHandler)
            throw new InvalidOperationException($"The service '{typeof(THandler).Name}' must derive from {nameof(EngineHandler)}.");

        foreach (var configure in _configurations)
            configure(serviceProvider, handler, engine);

        engine.ServicesManager.RegisterHandler(engineHandler);
    }
}