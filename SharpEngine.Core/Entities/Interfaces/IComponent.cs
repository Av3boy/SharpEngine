using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.Interfaces;

public interface IComponent
{
}

public static class GameObjectExtensions
{
    public static TComponent? GetComponentOrDefault<TComponent>(this GameObject gameObject) where TComponent : IComponent
    {
        var component = gameObject.Components.OfType<TComponent>().FirstOrDefault();
        return component;
    }

    public static T Create<T>(T instance) where T : GameObject
    {
        instance.OnCreated();
        return instance;
    }

    public static T Create<T>(Action<T>? configure = null) where T : GameObject, new()
    {
        var instance = new T();
        configure?.Invoke(instance);
        instance.OnCreated();
        return instance;
    }
}
