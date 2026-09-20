using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Scenes;

public static class SceneExtensions
{
    /// <summary>
    ///     Gets all the objects in the scene of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the objects to be retrieved.</typeparam>
    /// <returns>All the game objects in the current scene.</returns>
    public static List<T> GetObjectsOfType<T>(this SceneNode root)
    {
        // Existing API kept for compatibility. Internally uses the iterator implementation.
        return root.GetObjectsOfTypeEnumerable<T>().ToList();
    }

    /// <summary>
    ///     Iterates over the scene tree and yields objects of type <typeparamref name="T"/>.
    ///     This avoids allocating large temporary lists during traversal when callers can stream results.
    /// </summary>
    public static IEnumerable<T> GetObjectsOfTypeEnumerable<T>(this SceneNode root)
    {
        // Skip the root itself to preserve original behaviour (which iterated root.Children)
        foreach (var node in root.Children)
        {
            foreach (var child in GetObjectsOfTypeEnumerableImpl<T>(node))
                yield return child;
        }
    }

    private static IEnumerable<T> GetObjectsOfTypeEnumerableImpl<T>(SceneNode node)
    {
        if (node is T obj)
            yield return obj;

        foreach (var child in node.Children)
        {
            foreach (var descendant in GetObjectsOfTypeEnumerableImpl<T>(child))
                yield return descendant;
        }
    }

    /// <summary>
    ///     Iterates over the given <paramref name="elements"/> and performs the given <paramref name="action"/> for each element.
    /// </summary>
    /// <typeparam name="TEntityType">The type of the element.</typeparam>
    /// <param name="elements">The scene nodes to iterate over</param>
    /// <param name="action">The action to be executed for each element.</param>
    public static void Iterate<TEntityType>(this List<TEntityType> elements, Action<TEntityType> action) where TEntityType : SceneNode
    {
        foreach (var entity in elements)
        {
            action.Invoke(entity);

            var children = entity.Children.OfType<TEntityType>().ToList();
            if (children.Count != 0)
                Iterate(children, action);
        }
    }

    /// <summary>
    ///     Iterates over the given <paramref name="elements"/> and performs the given <paramref name="action"/> for each element.
    /// </summary>
    /// <typeparam name="TEntityType">The type of the element.</typeparam>
    /// <param name="elements">The scene nodes to iterate over</param>
    /// <param name="action">The action to be executed for each element.</param>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public static IEnumerable<Task> IterateAsync<TEntityType>(this IEnumerable<SceneNode> elements, Func<SceneNode, Task> action)
    {
        var tasks = new List<Task>();

        foreach (var entity in elements)
        {
            tasks.Add(action(entity));
            tasks.AddRange(IterateAsync<SceneNode>(entity.Children, action));
        }

        return tasks;
    }
}
