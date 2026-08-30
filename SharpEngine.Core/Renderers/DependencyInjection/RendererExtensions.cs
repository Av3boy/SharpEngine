using Microsoft.Extensions.DependencyInjection;

namespace SharpEngine.Core.Renderers.DependencyInjection;

/// <summary>
///   Provides extension methods for registering renderers in the dependency injection container.
/// </summary>
public static class RendererExtensions
{
    /// <summary>
    ///     Registers a renderer of type <typeparamref name="TRenderer"/> in the dependency injection container.
    /// </summary>
    /// <typeparam name="TRenderer">The type of the renderer to register.</typeparam>
    /// <param name="services">The service collection to add the renderer to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRenderer<TRenderer>(this IServiceCollection services) where TRenderer : RendererBase 
        => services.AddSingleton<RendererBase, TRenderer>();
}
