using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharpEngine.Core.DependencyInjection;

/// <summary>
///     Represents a Dependency Injection builder for configuring services and building the application.
/// </summary>
public class AppBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private IConfiguration? _configuration;

    private App? App;

    /// <summary>
    ///     Configures the application using the specified configuration action.
    /// </summary>
    /// <param name="configure">An action to configure the configuration builder.</param>
    /// <returns>The current <see cref="AppBuilder"/> instance for method chaining.</returns>
    public AppBuilder Configure(Action<IConfigurationBuilder> configure)
    {
        var configBuilder = new ConfigurationBuilder();
        configure(configBuilder);

        Configure(configBuilder);
        return this;
    }

    private void Configure(ConfigurationBuilder? configBuilder = null)
    {
        var builder = configBuilder ?? new ConfigurationBuilder();

        _configuration = builder.Build();
        _services.AddSingleton<IConfiguration>(_configuration);
    }

    /// <summary>
    ///     Configures the application services using the specified configuration action.
    /// </summary>
    /// <param name="configure">The configuration action to apply to the service collection.</param>
    /// <returns>The current <see cref="AppBuilder"/> instance for method chaining.</returns>
    public AppBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    /// <summary>
    ///     Builds the App instance if not already built.
    /// </summary>
    /// <remarks>
    ///     This method lazily initializes and caches the App instance.
    /// </remarks>
    /// <returns>The App instance.</returns>
    public App Build()
    {
        if (App != null)
            return App;

        if (_configuration == null)
            Configure();

        App = new App(_services.BuildServiceProvider(), _configuration!);
        return App;
    }
}