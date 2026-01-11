using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DITesting;

public class AppBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private IConfiguration? _configuration;

    private App App;

    public AppBuilder Configure(Action<IConfigurationBuilder> configure)
    {
        var configBuilder = new ConfigurationBuilder();
        configure(configBuilder);

        _configuration = configBuilder.Build();
        _services.AddSingleton<IConfiguration>(_configuration);

        return this;
    }

    public AppBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    public App Build()
    {
        if (_configuration == null)
            throw new InvalidOperationException("Configuration must be set before building the app.");

        App = new App(_services.BuildServiceProvider(), _configuration);
        return App;
    }
}
