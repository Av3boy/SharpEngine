using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharpEngine.Core.DependencyInjection;

public static class EngineServiceCollectionExtensions
{
    public static IServiceCollection AddEngine(this IServiceCollection services, Action<EngineRegistrationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new EngineRegistrationBuilder(services);
        configure(builder);

        services.AddSingleton<Engine>(serviceProvider =>
        {
            var engine = new Engine();
            builder.Apply(serviceProvider, engine);
            return engine;
        });

        return services;
    }
}
