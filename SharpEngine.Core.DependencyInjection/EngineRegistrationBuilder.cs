using Microsoft.Extensions.DependencyInjection;
using SharpEngine.Core.Handlers;
using System;
using System.Collections.Generic;

namespace SharpEngine.Core.DependencyInjection;

public sealed class EngineRegistrationBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<Type, object> _handlerRegistrations = [];

    public EngineRegistrationBuilder(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        _services = services;
    }

    public HandlerRegistrationBuilder<THandler> AddHandler<THandler>() where THandler : class
    {
        if (_handlerRegistrations.TryGetValue(typeof(THandler), out var existing))
            return (HandlerRegistrationBuilder<THandler>)existing;

        _services.AddSingleton<THandler>();

        var registration = new HandlerRegistrationBuilder<THandler>();
        _handlerRegistrations.Add(typeof(THandler), registration);
        return registration;
    }

    internal void Apply(IServiceProvider serviceProvider, Engine engine)
    {
        foreach (var registration in _handlerRegistrations.Values)
            ((IHandlerRegistration)registration).Apply(serviceProvider, engine);
    }
}